using FluentValidation;
using MercadoPago.Client.Payment;
using MercadoPago.Config;
using MercadoPago.Resource.Payment;
using Microsoft.Extensions.Options;
using Web.Application.Interfaces;
using Web.Domain.DTOs.MercadoPago;
using Web.Domain.DTOs.Pedidos;
using Web.Domain.Entities;
using Web.Domain.Enums;
using Web.Domain.Interfaces;

namespace Web.Application.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IProdutoRepository _produtoRepository;
        private readonly IEstabelecimentoService _estabelecimentoService;
        private readonly IMovimentacaoEstoqueService _movimentacaoEstoqueService;
        private readonly IPagamentoService _pagamentoService;
        private readonly IValidator<Pedido> _pedidoValidator;
        private readonly IOptions<MercadoPagoSettings> _mpSettings;

        public PedidoService(
          IPedidoRepository pedidoRepository,
            IProdutoRepository produtoRepository,
             IMovimentacaoEstoqueService movimentacaoEstoqueService,
              IPagamentoService pagamentoService,
            IValidator<Pedido> pedidoValidator,
          IOptions<MercadoPagoSettings> mpSettings,
         IEstabelecimentoService estabelecimentoService)
        {
            _pedidoRepository = pedidoRepository;
            _produtoRepository = produtoRepository;
            _movimentacaoEstoqueService = movimentacaoEstoqueService;
            _pagamentoService = pagamentoService;
            _pedidoValidator = pedidoValidator;
            _mpSettings = mpSettings;
            _estabelecimentoService = estabelecimentoService;
        }

        public async Task<Pedido?> FinalizarPedidoAsync(PedidoCreateDto pedidoDto, string usuarioId)
        {
            // 1) Criar Pedido
            var pedido = new Pedido
            {
                UsuarioId = usuarioId,
                EstabelecimentoId = pedidoDto.EstabelecimentoId,
                EnderecoEntrega = pedidoDto.EnderecoEntrega,
                FormaPagamento = pedidoDto.FormaPagamento,
                ExternalReference = Guid.NewGuid().ToString(), // Gera uma referência única, para nosso webhook mercado pago.

                Itens = pedidoDto.Itens.Select(itemDto => new ItemPedido
                {
                    ProdutoId = itemDto.ProdutoId,
                    Quantidade = itemDto.Quantidade,
                    Observacao = itemDto.Observacao
                }).ToList()
            };

            // 2) Atualizar Estoque, e Obter preços dos Produtos para o pedido, antes disso validemos tudo
            decimal totalDoPedido = 0m;
            foreach (var item in pedido.Itens)
            {
                var produto = await _produtoRepository.GetByIdAsync(item.ProdutoId);
                if (produto == null)
                {
                    throw new Exception("Produto não encontrado no estoque");
                }
                if (produto.QuantidadeEmEstoque < item.Quantidade)
                {
                    throw new Exception("Quantidade insuficiente do produto no estoque");
                }

                // 3 ) Atribuir os valores
                item.PrecoUnitario = produto.Preco;
                item.Subtotal = item.PrecoUnitario * item.Quantidade;

                //reservar nosso produto com um subtrção provisoria .
                produto.QuantidadeEmEstoque -= item.Quantidade;
                await _produtoRepository.UpdateAsync(produto);

                totalDoPedido += item.Subtotal.Value;
            }

            // pegar taxa de entrega para montar valor total
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(pedidoDto.EstabelecimentoId);
            if (estabelecimento == null)
            {
                throw new Exception("Estabelecimento não encontrado");
            }

            var taxaEntrega = estabelecimento.TaxaEntregaFixa ?? 0m;

            // calcular nosso pedido e setar nosso valor
            pedido.ValorTotal = totalDoPedido;
            pedido.TaxaEntrega = taxaEntrega;
            pedido.ValorTotal += taxaEntrega;

            // 4 ) Inserir Nosso Pedido
            pedido.AtualizarStatus(StatusPedido.AguardandoPagamento);
            await _pedidoRepository.AddAsync(pedido);

            return pedido;
        }

        public async Task AddPedidoAsync(Pedido pedido)
        {
            var validationResult = await _pedidoValidator.ValidateAsync(pedido);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // Chama o repositório para lidar com a lógica de transação
            await _pedidoRepository.AddPedidoComTransacaoAsync(pedido, pedido.Itens);
        }

        public async Task ConfirmarPedidoAsync(int pedidoId)
        {
            using var transaction = await _pedidoRepository.BeginTransactionAsync();
            try
            {
                var pedido = await _pedidoRepository.GetByIdAsync(pedidoId);
                if (pedido == null)
                    throw new Exception("Pedido não foi encontrado!");

                if (pedido.Status != StatusPedido.Criado)
                    throw new Exception("O pedido não está com o status de criado, então não pode ser confirmado");

                foreach (var item in pedido.Itens)
                {
                    var produto = await _produtoRepository.GetByIdAsync(item.ProdutoId);
                    if (produto == null)
                    {
                        throw new Exception($"Produto com id {item.ProdutoId} não encontrado");
                    }

                    var estoqueDisponivel = produto.QuantidadeEmEstoque - produto.QuantidadeReservada;

                    if (estoqueDisponivel < item.Quantidade)
                    {
                        throw new Exception($"Estoque Insuficiente do Produto {produto.Nome} ao Confirmar o Pedido.");
                    }
                }

                // atualizar o status
                pedido.AtualizarStatus(StatusPedido.Confirmado);
                await _pedidoRepository.UpdateAsync(pedido);
                // registrar movimentação
                foreach (var item in pedido.Itens)
                {
                    var produto = await _produtoRepository.GetByIdAsync(item.ProdutoId);

                    //atualizar nosso estoque real
                    produto.QuantidadeEmEstoque -= item.Quantidade;
                    produto.QuantidadeReservada -= item.Quantidade;

                    await _produtoRepository.UpdateAsync(produto);

                    // Registrar movimentação
                    var movimentacao = new MovimentacaoEstoque
                    {
                        ProdutoId = item.ProdutoId,
                        Quantidade = item.Quantidade,
                        Tipo = TipoMovimentacao.Saida,
                        Observacao = $"Venda - Pedido {pedido.Id}",
                        DataMovimentacao = DateTime.UtcNow
                    };
                    await _movimentacaoEstoqueService.AddMovimentacaoAsync(movimentacao);
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task CancelarPedidoAsync(int pedidoId)
        {
            var pedido = await _pedidoRepository.GetByIdAsync(pedidoId);
            if (pedido == null)
                throw new Exception("Pedido não foi encontrado!");

            if (pedido.Status != StatusPedido.Criado)
                throw new Exception("Somente pedidos com status `Criado` podem ser cancelados.");

            // atualizar status para cancelado
            pedido.AtualizarStatus(StatusPedido.Cancelado);
            await _pedidoRepository.UpdateAsync(pedido);

            //retornar o produto para o estoque , antes reservado
            foreach (var item in pedido.Itens)
            {
                var produto = await _produtoRepository.GetByIdAsync(item.ProdutoId);

                if (produto != null)
                {
                    produto.QuantidadeEmEstoque += item.Quantidade;
                    await _produtoRepository.UpdateAsync(produto);
                }
            }
        }

        public async Task AtualizarStatusPedidoAsync(int pedidoId, StatusPedido novoStatus)
        {
            var pedido = await _pedidoRepository.GetByIdAsync(pedidoId);
            if (pedido == null)
                throw new Exception("Pedido não encontrado.");
            pedido.AtualizarStatus(novoStatus);
            await _pedidoRepository.UpdateAsync(pedido);
        }

        public async Task<Pedido> GetPedidoByIdAsync(int id)
        {
            return await _pedidoRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Pedido>> GetPedidosByUsuarioIdAsync(string usuarioId)
        {
            return await _pedidoRepository.GetByUserIdAsync(usuarioId);
        }

        public async Task<IEnumerable<Pedido>> GetPedidosByEstabelecimentoIdAsync(int estabelecimentoId)
        {
            return await _pedidoRepository.GetByEstabelecimentoIdAsync(estabelecimentoId);
        }

        public async Task UpdatePedidoAsync(Pedido pedido)
        {
            await _pedidoRepository.UpdateAsync(pedido);
        }

        public async Task UpdateStatusPedidoAsync(int pedidoId, StatusPedido novoStatus)
        {
            var pedido = await _pedidoRepository.GetByIdAsync(pedidoId);
            if (pedido != null)
            {
                pedido.AtualizarStatus(novoStatus);
                await _pedidoRepository.UpdateAsync(pedido);
            }
        }

        public async Task<Pedido> GetPedidoByExternalReferenceAsync(string externalReference)
        {
            return await _pedidoRepository.GetByExternalReferenceAsync(externalReference);
        }

        public async Task<Payment> ObterPagamentoDoMercadoPagoAsync(string paymentId)
        {
            MercadoPagoConfig.AccessToken = _mpSettings.Value.AccessToken;
            var client = new PaymentClient();
            long paymentIdLong;
            if (!long.TryParse(paymentId, out paymentIdLong))
            {
                throw new ArgumentException("paymentId deve ser um número válido.");
            }

            Payment payment = await client.GetAsync(paymentIdLong);
            return payment;
        }
    }
}