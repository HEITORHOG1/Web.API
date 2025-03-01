using MercadoPago.Resource.Payment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Web.API.Services;
using Web.Application.Interfaces;
using Web.Domain.DTOs.MercadoPago;
using Web.Domain.Enums;

namespace Web.API.Controllers
{
    /// <summary>
    /// Controller para lidar com pagamentos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PagamentoController : ControllerBase
    {
        private readonly ILogger<PagamentoController> _logger;
        private readonly IPedidoService _pedidoService;
        private readonly IMercadoPagoService _mercadoPagoService;
        private readonly IOptions<MercadoPagoSettings> _mpSettings;
        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="pedidoService"></param>
        /// <param name="mercadoPagoService"></param>
        /// <param name="mpSettings"></param>
        public PagamentoController(ILogger<PagamentoController> logger, IPedidoService pedidoService, IMercadoPagoService mercadoPagoService, IOptions<MercadoPagoSettings> mpSettings)
        {
            _logger = logger;
            _pedidoService = pedidoService;
            _mercadoPagoService = mercadoPagoService;
            _mpSettings = mpSettings;
        }

        /// <summary>
        /// Endpoint para receber notificações do Mercado Pago.
        /// </summary>
        /// <param name="id">ID do pagamento.</param>
        /// <param name="topic">Tipo de notificação.</param>
        /// <returns>Status 200 OK.</returns>
        [HttpPost("notificacoes")]
        public async Task<IActionResult> Notificacoes([FromQuery] string id, [FromQuery] string topic)
        {
            _logger.LogInformation($"Notificação recebida. ID: {id}, Tópico: {topic}");

            if (topic != "payment")
            {
                _logger.LogWarning($"Tópico desconhecido: {topic}");
                return BadRequest("Tópico desconhecido.");
            }

            try
            {
                // 1) Obter detalhes do pagamento via MercadoPagoService
                Payment payment = await _mercadoPagoService.ObterPagamentoDoMercadoPagoAsync(id);

                // 2) Encontrar o pedido relacionado usando ExternalReference
                var pedido = await _pedidoService.GetPedidoByExternalReferenceAsync(payment.ExternalReference);

                if (pedido == null)
                {
                    _logger.LogError($"Pedido não encontrado com ExternalReference: {payment.ExternalReference}");
                    return NotFound("Pedido não encontrado.");
                }

                // 3) Atualizar o status do pedido com base no status do pagamento
                switch (payment.Status)
                {
                    case "approved":
                        await _pedidoService.AtualizarStatusPedidoAsync(pedido.Id, StatusPedido.PagamentoAprovado);
                        break;

                    case "pending":
                        await _pedidoService.AtualizarStatusPedidoAsync(pedido.Id, StatusPedido.Pendente);
                        break;

                    case "in_process":
                        await _pedidoService.AtualizarStatusPedidoAsync(pedido.Id, StatusPedido.EmProcessamento);
                        break;

                    case "rejected":
                        await _pedidoService.AtualizarStatusPedidoAsync(pedido.Id, StatusPedido.Cancelado);
                        break;

                    default:
                        _logger.LogWarning($"Status desconhecido do pagamento: {payment.Status}");
                        break;
                }

                _logger.LogInformation($"Status do pedido {pedido.Id} atualizado para {payment.Status}.");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao processar notificação: {ex.Message}");
                return StatusCode(500, "Erro interno.");
            }
        }

        /// <summary>
        /// Endpoint para tratar retorno do pagamento no front-end.
        /// </summary>
        /// <param name="externalReference">External Reference do pedido.</param>
        /// <returns>Mensagem de sucesso ou falha.</returns>
        [HttpGet("sucesso")]
        public IActionResult Sucesso([FromQuery] string externalReference)
        {
            // Implementar a lógica para mostrar uma página de sucesso ao usuário
            return Ok(new { Message = "Pagamento aprovado com sucesso!", ExternalReference = externalReference });
        }
        /// <summary>
        /// Endpoint para tratar retorno do pagamento no front-end.
        /// </summary>
        /// <param name="externalReference"></param>
        /// <returns></returns>
        [HttpGet("falha")]
        public IActionResult Falha([FromQuery] string externalReference)
        {
            // Implementar a lógica para mostrar uma página de falha ao usuário
            return Ok(new { Message = "Pagamento falhou.", ExternalReference = externalReference });
        }
        /// <summary>
        /// Endpoint para tratar retorno do pagamento no front-end.
        /// </summary>
        /// <param name="externalReference"></param>
        /// <returns></returns>
        [HttpGet("pendente")]
        public IActionResult Pendente([FromQuery] string externalReference)
        {
            // Implementar a lógica para mostrar uma página de pagamento pendente ao usuário
            return Ok(new { Message = "Pagamento pendente.", ExternalReference = externalReference });
        }
    }
}