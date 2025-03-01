using FluentValidation;
using Web.Application.Interfaces;
using Web.Application.Validators;
using Web.Domain.Entities;
using Web.Domain.Interfaces;

namespace Web.Application.Services
{
    public class CarrinhoService : ICarrinhoService
    {
        private readonly ICarrinhoRepository _carrinhoRepository;
        private readonly IProdutoRepository _produtoRepository;
        private readonly CarrinhoItemValidator _validationRules;

        public CarrinhoService(ICarrinhoRepository carrinhoRepository, IProdutoRepository produtoRepository, CarrinhoItemValidator validationRules)
        {
            _carrinhoRepository = carrinhoRepository;
            _produtoRepository = produtoRepository;
            _validationRules = validationRules;
        }

        public async Task<IEnumerable<CarrinhoItem>> GetCarrinhoItensAsync(string usuarioId)
        {
            return await _carrinhoRepository.GetCarrinhoItensByUsuarioIdAsync(usuarioId);
        }

        public async Task AddItemAoCarrinhoAsync(string usuarioId, int estabelecimentoId, int produtoId, int quantidade)
        {
            // Validar o produto
            var produto = await _produtoRepository.GetByIdAsync(produtoId);
            if (produto == null)
            {
                throw new Exception("Produto não encontrado.");
            }

            // Verificar se há estoque suficiente
            if (produto.QuantidadeEmEstoque < quantidade)
            {
                throw new Exception("Estoque insuficiente para o produto selecionado.");
            }

            // Verificar se o item já existe no carrinho
            var carrinhoItem = await _carrinhoRepository.GetItemAsync(usuarioId, produtoId, estabelecimentoId);
            if (carrinhoItem != null)
            {
                // Incrementar a quantidade
                carrinhoItem.Quantidade += quantidade;
                await _carrinhoRepository.UpdateAsync(carrinhoItem);
            }
            else
            {
                // Adicionar novo item ao carrinho
                carrinhoItem = new CarrinhoItem
                {
                    UsuarioId = usuarioId,
                    EstabelecimentoId = estabelecimentoId,
                    ProdutoId = produtoId,
                    Quantidade = quantidade,
                    DataAdicionado = DateTime.UtcNow
                };
                await _carrinhoRepository.AddAsync(carrinhoItem);
            }
        }

        public async Task AtualizarQuantidadeAsync(string usuarioId, int produtoId, int quantidade)
        {
            // Validar a quantidade
            if (quantidade <= 0)
            {
                throw new Exception("A quantidade deve ser maior que zero.");
            }

            // Verificar se o item existe no carrinho
            var carrinhoItem = await _carrinhoRepository.GetCarrinhoItemAsync(usuarioId, produtoId);
            if (carrinhoItem == null)
            {
                throw new Exception("Item não encontrado no carrinho.");
            }

            // Verificar se há estoque suficiente
            var produto = await _produtoRepository.GetByIdAsync(produtoId);
            if (produto == null || produto.QuantidadeEmEstoque < quantidade)
            {
                throw new Exception("Estoque insuficiente para o produto selecionado.");
            }

            // Atualizar a quantidade
            carrinhoItem.Quantidade = quantidade;
            await _carrinhoRepository.UpdateAsync(carrinhoItem);
        }

        public async Task RemoverItemDoCarrinhoAsync(string usuarioId, int produtoId)
        {
            var carrinhoItem = await _carrinhoRepository.GetCarrinhoItemAsync(usuarioId, produtoId);
            if (carrinhoItem != null)
            {
                await _carrinhoRepository.DeleteAsync(carrinhoItem.Id);
            }
        }

        public async Task LimparCarrinhoAsync(string usuarioId)
        {
            var itens = await _carrinhoRepository.GetCarrinhoItensByUsuarioIdAsync(usuarioId);
            foreach (var item in itens)
            {
                await _carrinhoRepository.DeleteAsync(item.Id);
            }
        }

        public async Task<CarrinhoItem?> GetItemAsync(string userId, int produtoId, int estabelecimentoId)
        {
            return await _carrinhoRepository.GetItemAsync(userId, produtoId, estabelecimentoId);
        }

        public async Task AtualizarItemAsync(CarrinhoItem carrinhoItem)
        {
            if (carrinhoItem == null)
            {
                throw new ArgumentNullException(nameof(carrinhoItem), "O item do carrinho não pode ser nulo.");
            }

            // Validação adicional
            var validationResult = _validationRules.Validate(carrinhoItem);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // Verificar se o item existe no carrinho
            var itemExistente = await _carrinhoRepository.GetItemAsync(carrinhoItem.UsuarioId, carrinhoItem.ProdutoId, carrinhoItem.EstabelecimentoId);
            if (itemExistente == null)
            {
                throw new Exception("Item não encontrado no carrinho.");
            }

            // Atualizar o item
            itemExistente.Quantidade = carrinhoItem.Quantidade;
            itemExistente.Status = carrinhoItem.Status;
            await _carrinhoRepository.UpdateAsync(itemExistente);
        }

        public async Task AdicionarItensAoCarrinhoAsync(string usuarioId, List<CarrinhoItem> itens)
        {
            await _carrinhoRepository.AdicionarItensAoCarrinhoAsync(usuarioId, itens);
        }

        public async Task AtualizarItensNoCarrinhoAsync(string usuarioId, List<CarrinhoItem> itens)
        {
            await _carrinhoRepository.AtualizarItensNoCarrinhoAsync(usuarioId, itens);
        }

        public async Task RemoverItensDoCarrinhoAsync(string usuarioId, List<int> produtoIds)
        {
            await _carrinhoRepository.RemoverItensDoCarrinhoAsync(usuarioId, produtoIds);
        }
    }
}