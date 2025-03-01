using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.API.Services;
using Web.Application.Interfaces;
using Web.Domain.DTOs;
using Web.Domain.DTOs.Pedidos;
using Web.Domain.Entities;
using Web.Domain.Enums;

namespace Web.API.Controllers
{
    /// <summary>
    /// Controlador responsável pelas operações que os clientes podem realizar.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Cliente")]
    public class ClienteController : ControllerBase
    {
        private readonly IProdutoService _produtoService;
        private readonly ICategoriaService _categoriaService;
        private readonly ICarrinhoService _carrinhoService;
        private readonly IPedidoService _pedidoService;
        private readonly IEntregaService _entregaService;
        private readonly IEstabelecimentoService _estabelecimentoService;
        private readonly IMercadoPagoService _mercadoPagoService;

        /// <summary>
        /// Construtor do ClienteController com injeção de dependências.
        /// </summary>
        /// <param name="produtoService">Serviço para manipulação de produtos.</param>
        /// <param name="categoriaService">Serviço para manipulação de categorias.</param>
        /// <param name="carrinhoService">Serviço para manipulação do carrinho de compras.</param>
        /// <param name="pedidoService">Serviço para manipulação de pedidos.</param>
        public ClienteController(
            IProdutoService produtoService,
            ICategoriaService categoriaService,
            ICarrinhoService carrinhoService,
            IPedidoService pedidoService,
            IEntregaService entregaService,
            IEstabelecimentoService estabelecimentoService,
            IMercadoPagoService mercadoPagoService)
        {
            _produtoService = produtoService;
            _categoriaService = categoriaService;
            _carrinhoService = carrinhoService;
            _pedidoService = pedidoService;
            _entregaService = entregaService;
            _estabelecimentoService = estabelecimentoService;
            _mercadoPagoService = mercadoPagoService;
        }

        /// <summary>
        /// Obtém a lista de todas as categorias disponíveis.
        /// </summary>
        /// <param name="estabelecimentoId">ID do estabelecimento.</param>
        /// <returns>Lista de categorias.</returns>
        ///
        [Authorize(Policy = "ViewCategorias")]
        [HttpGet("estabelecimentos/{estabelecimentoId:int}/categorias")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCategorias(int estabelecimentoId)
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }
            // Verificar se o estabelecimento existe
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(estabelecimentoId);
            if (estabelecimento == null)
            {
                return NotFound("Estabelecimento não encontrado");
            }

            var categorias = await _categoriaService.GetCategoriasByEstabelecimentoIdAsync(estabelecimentoId);
            return Ok(categorias);
        }

        /// <summary>
        /// Obtém os produtos de uma categoria específica com paginação.
        /// </summary>
        /// <param name="estabelecimentoId">ID do estabelecimento.</param>
        /// <param name="categoriaId">ID da categoria.</param>
        /// <param name="pageNumber">Número da página (padrão: 1).</param>
        /// <param name="pageSize">Tamanho da página (padrão: 10).</param>
        /// <returns>Lista paginada de produtos.</returns>
        [Authorize(Policy = "ViewProdutos")]
        [HttpGet("estabelecimentos/{estabelecimentoId:int}/categorias/{categoriaId:int}/produtos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetProdutosByCategoria(int estabelecimentoId, int categoriaId, int pageNumber = 1, int pageSize = 10)
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }
            // Verificar se o estabelecimento existe
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(estabelecimentoId);
            if (estabelecimento == null)
            {
                return NotFound("Estabelecimento não encontrado");
            }

            var produtos = await _produtoService.GetProdutosByCategoriaIdAsync(estabelecimentoId, categoriaId);
            return Ok(produtos);
        }

        /// <summary>
        /// Obtém os detalhes de um produto específico.
        /// </summary>
        /// <param name="estabelecimentoId">ID do estabelecimento.</param>
        /// <param name="produtoId">ID do produto.</param>
        /// <returns>Detalhes do produto.</returns>
        [Authorize(Policy = "ViewProdutos")]
        [HttpGet("estabelecimentos/{estabelecimentoId:int}/produtos/{produtoId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetProdutoById(int estabelecimentoId, int produtoId)
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }
            // Verificar se o estabelecimento existe
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(estabelecimentoId);
            if (estabelecimento == null)
            {
                return NotFound("Estabelecimento não encontrado");
            }

            var produto = await _produtoService.GetProdutoByIdAsync(estabelecimentoId, produtoId);
            if (produto == null)
            {
                return NotFound("Produto não encontrado");
            }
            return Ok(produto);
        }

        /// <summary>
        /// Finaliza a compra de um pedido.
        /// </summary>
        /// <param name="compraDto"></param>
        /// <returns></returns>
        [HttpPost("finalizar-compra")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> FinalizarCompra([FromBody] FinalizarCompraDto compraDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 1) Verifica se o usuário está autenticado
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }

            // 2) Obtém os itens do carrinho do usuário e filtra apenas itens ativos
            //var carrinhoItens = await _carrinhoService.GetCarrinhoItensAsync(userId);
            var itensAtivos = compraDto.Itens;
            if (!itensAtivos.Any())
            {
                return BadRequest("O carrinho está vazio.");
            }

            // 5) Carrega o estabelecimento para obter a TaxaEntregaFixa
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(compraDto.EstabelecimentoId);
            if (estabelecimento == null)
            {
                return NotFound("Estabelecimento não encontrado.");
            }

            // Usa a taxa de entrega cadastrada no Estabelecimento (caso seja nula, assume 0)
            var taxaEntrega = estabelecimento.TaxaEntregaFixa ?? 0m;

            // 6) Monta o objeto Pedido
            var pedido = new Pedido
            {
                UsuarioId = userId,
                EstabelecimentoId = compraDto.EstabelecimentoId,
                EnderecoEntrega = compraDto.EnderecoEntrega,
                FormaPagamento = compraDto.FormaPagamento,
                TaxaEntrega = taxaEntrega,
                ExternalReference = Guid.NewGuid().ToString(), // Gera uma referência única
                ValorTotal = compraDto.ValorTotal,
                Itens = itensAtivos.Select(ci => new ItemPedido
                {
                    ProdutoId = ci.ProdutoId,
                    Quantidade = ci.Quantidade,
                    PrecoUnitario = ci.PrecoUnitario,
                    Subtotal = ci.Subtotal,
                }).ToList()
            };

            // 7) Define o status inicial do pedido
            pedido.AtualizarStatus(StatusPedido.AguardandoPagamento);

            // 8) Salva o pedido no banco
            await _pedidoService.AddPedidoAsync(pedido);

            // 9) Limpa o carrinho
            await _carrinhoService.LimparCarrinhoAsync(userId);

            // 10) Cria preferência no Mercado Pago e obtém a URL para pagamento
            var pagamentoUrl = await _mercadoPagoService.CriarPreferenciaAsync(pedido);

            // 11) Retorna para o front-end a URL de pagamento
            return Ok(new
            {
                Message = "Pedido criado. Redirecione para o pagamento.",
                PedidoId = pedido.Id,
                PagamentoUrl = pagamentoUrl
            });
        }

        /// <summary>
        /// Lista todos os pedidos realizados pelo cliente autenticado.
        /// </summary>
        /// <returns>Lista de pedidos.</returns>
        [Authorize(Policy = "ViewPedidos")]
        [HttpGet("pedidos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ListarPedidos()
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }
            var pedidos = await _pedidoService.GetPedidosByUsuarioIdAsync(userId);
            return Ok(pedidos);
        }

        /// <summary>
        /// Obtém os detalhes de um pedido específico do cliente.
        /// </summary>
        /// <param name="pedidoId">ID do pedido.</param>
        /// <returns>Detalhes do pedido.</returns>
        [Authorize(Policy = "ViewPedidos")]
        [HttpGet("pedidos/{pedidoId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AcompanharPedido(int pedidoId)
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }
            var pedido = await _pedidoService.GetPedidoByIdAsync(pedidoId);

            if (pedido == null || pedido.UsuarioId != userId)
                return NotFound("Pedido não encontrado ou você não tem permissão para acessá-lo.");

            return Ok(pedido);
        }

        /// <summary>
        /// Obtém os detalhes da entrega de um pedido específico do cliente.
        /// </summary>
        /// <param name="pedidoId">ID do pedido.</param>
        /// <returns>Detalhes da entrega.</returns>
        [Authorize(Policy = "ViewPedidos")]
        [HttpGet("pedidos/{pedidoId:int}/entrega")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AcompanharEntrega(int pedidoId)
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }
            var pedido = await _pedidoService.GetPedidoByIdAsync(pedidoId);

            if (pedido == null || pedido.UsuarioId != userId)
                return NotFound("Pedido não encontrado ou você não tem permissão para acessá-lo.");

            var entrega = await _entregaService.GetByPedidoIdAsync(pedidoId);

            if (entrega == null)
                return NotFound("Entrega não encontrada para este pedido.");

            return Ok(entrega);
        }
    }
}