using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.Application.Interfaces;
using Web.Domain.DTOs.Pedidos;
using Web.Domain.Entities;
using Web.Domain.Enums;

namespace Web.API.Controllers
{
    /// <summary>
    /// Controller para gerenciar pedidos.
    /// </summary>
    [Route("api/pedidos")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;
        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="pedidoService"></param>
        public PedidoController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        /// <summary>
        /// Cria um novo pedido.
        /// </summary>
        /// <param name="pedidoDto">Objeto contendo os dados para criar um novo pedido.</param>
        /// <returns>Retorna o pedido criado com o status 201.</returns>
        /// <response code="201">Pedido criado com sucesso.</response>
        /// <response code="401">Usuário não autenticado.</response>
        /// <response code="400">Erro ao processar a solicitação.</response>
        [Authorize(Roles = "Cliente")]
        [HttpPost]
        [ProducesResponseType(typeof(Pedido), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarPedido([FromBody] PedidoCreateDto pedidoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }

            var pedido = new Pedido
            {
                UsuarioId = userId,
                EstabelecimentoId = pedidoDto.EstabelecimentoId,
                EnderecoEntrega = pedidoDto.EnderecoEntrega,
                FormaPagamento = pedidoDto.FormaPagamento,
                DataCriacao = DateTime.UtcNow
            };

            foreach (var itemDto in pedidoDto.Itens)
            {
                var item = new ItemPedido
                {
                    ProdutoId = itemDto.ProdutoId,
                    Quantidade = itemDto.Quantidade,
                    Observacao = itemDto.Observacao
                };
                pedido.Itens.Add(item);
            }

            pedido.AtualizarStatus(StatusPedido.Criado);

            await _pedidoService.AddPedidoAsync(pedido);

            return CreatedAtAction(nameof(GetById), new { id = pedido.Id }, pedido);
        }

        /// <summary>
        /// Obtém os detalhes de um pedido específico pelo ID.
        /// </summary>
        /// <param name="id">ID do pedido.</param>
        /// <returns>Retorna os detalhes do pedido.</returns>
        /// <response code="200">Pedido encontrado.</response>
        /// <response code="404">Pedido não encontrado.</response>
        [Authorize(Roles = "Proprietario,Gerente")]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Pedido), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }
            var pedido = await _pedidoService.GetPedidoByIdAsync(id);
            if (pedido == null)
            {
                return NotFound("Pedido não encontrado");
            }

            return Ok(pedido);
        }

        /// <summary>
        /// Confirma um pedido.
        /// </summary>
        /// <param name="id">ID do pedido.</param>
        /// <returns>Retorna um status 204 se a confirmação for bem-sucedida.</returns>
        /// <response code="204">Pedido confirmado com sucesso.</response>
        /// <response code="400">Erro ao confirmar o pedido.</response>
        [Authorize(Roles = "Proprietario,Gerente")]
        [HttpPost("{id:int}/confirmar")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmarPedido(int id)
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }
            try
            {
                await _pedidoService.ConfirmarPedidoAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Cancela um pedido.
        /// </summary>
        /// <param name="id">ID do pedido.</param>
        /// <returns>Retorna um status 204 se o cancelamento for bem-sucedido.</returns>
        /// <response code="204">Pedido cancelado com sucesso.</response>
        /// <response code="400">Erro ao cancelar o pedido.</response>
        [Authorize(Roles = "Cliente")]
        [HttpPost("{id:int}/cancelar")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CancelarPedido(int id)
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }
            try
            {
                await _pedidoService.CancelarPedidoAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza o status de um pedido.
        /// </summary>
        /// <param name="id">ID do pedido.</param>
        /// <param name="pedidoUpdateDto">Objeto contendo o novo status do pedido.</param>
        /// <returns>Retorna um status 204 se a atualização for bem-sucedida.</returns>
        /// <response code="204">Pedido atualizado com sucesso.</response>
        /// <response code="400">Erro ao atualizar o pedido.</response>
        /// <response code="404">Pedido não encontrado.</response>
        [Authorize(Roles = "Cliente,Proprietario,Gerente")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AtualizarPedido(int id, [FromBody] PedidoUpdateDto pedidoUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }
            if (id != pedidoUpdateDto.Id)
            {
                return BadRequest("O ID do pedido na URL e no corpo da requisição não correspondem.");
            }

            var pedidoExistente = await _pedidoService.GetPedidoByIdAsync(id);
            if (pedidoExistente == null)
            {
                return NotFound("Pedido não encontrado.");
            }

            try
            {
                pedidoExistente.AtualizarStatus(pedidoUpdateDto.Status);
                await _pedidoService.UpdatePedidoAsync(pedidoExistente);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}