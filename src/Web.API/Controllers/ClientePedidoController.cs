using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.API.Services;
using Web.Application.Interfaces;
using Web.Domain.DTOs;
using Web.Domain.Entities;
using Web.Domain.Enums;

namespace Web.API.Controllers
{
    /// <summary>
    /// Controlador responsável pelas operações que os clientes podem realizar.
    /// </summary>
    [Route("api/clientes/pedidos")]
    [ApiController]
    [Authorize(Roles = "Cliente")]
    public class ClientePedidoController : ControllerBase
    {
        private readonly ICarrinhoService _carrinhoService;
        private readonly IPedidoService _pedidoService;
        private readonly IEstabelecimentoService _estabelecimentoService;
        private readonly IMercadoPagoService _mercadoPagoService;

        /// <summary>
        /// Cria uma nova instância de <see cref="ClientePedidoController"/>.
        /// </summary>
        /// <param name="carrinhoService"></param>
        /// <param name="pedidoService"></param>
        /// <param name="estabelecimentoService"></param>
        /// <param name="mercadoPagoService"></param>
        public ClientePedidoController(
            ICarrinhoService carrinhoService,
            IPedidoService pedidoService,
            IEstabelecimentoService estabelecimentoService,
            IMercadoPagoService mercadoPagoService)
        {
            _carrinhoService = carrinhoService;
            _pedidoService = pedidoService;
            _estabelecimentoService = estabelecimentoService;
            _mercadoPagoService = mercadoPagoService;
        }

        /// <summary>
        /// Cria um novo pedido.
        /// </summary>
        /// <param name="pedidoDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] FinalizarCompraDto pedidoDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Usuário não autenticado");

            var itensAtivos = pedidoDto.Itens;
            if (!itensAtivos.Any())
                return BadRequest("O carrinho está vazio.");

            var estabelecimento = await _estabelecimentoService.GetByIdAsync(pedidoDto.EstabelecimentoId);
            if (estabelecimento == null)
                return NotFound("Estabelecimento não encontrado.");

            var pedido = new Pedido
            {
                UsuarioId = userId,
                EstabelecimentoId = pedidoDto.EstabelecimentoId,
                EnderecoEntrega = pedidoDto.EnderecoEntrega,
                FormaPagamento = pedidoDto.FormaPagamento,
                TaxaEntrega = estabelecimento.TaxaEntregaFixa ?? 0m,
                ExternalReference = Guid.NewGuid().ToString(),
                ValorTotal = pedidoDto.ValorTotal,
                Itens = itensAtivos.Select(ci => new ItemPedido
                {
                    ProdutoId = ci.ProdutoId,
                    Quantidade = ci.Quantidade,
                    PrecoUnitario = ci.PrecoUnitario,
                    Subtotal = ci.Subtotal,
                }).ToList()
            };

            pedido.AtualizarStatus(StatusPedido.AguardandoPagamento);
            await _pedidoService.AddPedidoAsync(pedido);
            await _carrinhoService.LimparCarrinhoAsync(userId);

            var pagamentoUrl = await _mercadoPagoService.CriarPreferenciaAsync(pedido);

            return Created($"api/clientes/pedidos/{pedido.Id}",
                new { PedidoId = pedido.Id, PagamentoUrl = pagamentoUrl });
        }
    }
}