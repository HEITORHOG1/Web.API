using Microsoft.AspNetCore.Mvc;
using Web.API.Services;
using Web.Domain.DTOs.MercadoPago;

namespace Web.API.Controllers
{
    /// <summary>
    /// Controlador para integração de pagamentos do Mercado Pago
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MercadoPagoController : ControllerBase
    {
        private readonly IMercadoPagoService _mercadoPagoService;
        private readonly ILogger<MercadoPagoController> _logger;

        public MercadoPagoController(IMercadoPagoService mercadoPagoService, ILogger<MercadoPagoController> logger)
        {
            _mercadoPagoService = mercadoPagoService;
            _logger = logger;
        }

        /// <summary>
        /// Configura um webhook para receber notificações do Mercado Pago.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [HttpPost("webhook/configurar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfigurarWebhook([FromBody] WebhookConfigDto config)
        {
            try
            {
                var webhookId = await _mercadoPagoService.RegistrarWebhookAsync(config.Url);
                return Ok(new { WebhookId = webhookId, Message = "Webhook configurado com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao configurar webhook");
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Verifica o status de um pagamento no Mercado Pago.
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        [HttpGet("pagamento/{paymentId}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> VerificarStatusPagamento(string paymentId)
        {
            try
            {
                var payment = await _mercadoPagoService.ObterPagamentoDoMercadoPagoAsync(paymentId);
                return Ok(new
                {
                    Status = payment.Status,
                    StatusDetail = payment.StatusDetail,
                    ExternalReference = payment.ExternalReference,
                    DateApproved = payment.DateApproved
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar status do pagamento {PaymentId}", paymentId);
                return NotFound(new { Message = "Pagamento não encontrado ou erro na consulta" });
            }
        }

        /// <summary>
        /// Lista os métodos de pagamento disponíveis para um país.
        /// </summary>
        /// <param name="pais"></param>
        /// <returns></returns>
        [HttpGet("metodos-pagamento")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ListarMetodosPagamento([FromQuery] string pais = "BR")
        {
            try
            {
                var metodos = await _mercadoPagoService.ObterMetodosPagamentoPorPaisAsync(pais);
                return Ok(metodos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar métodos de pagamento para país {Pais}", pais);
                return StatusCode(500, new { Message = "Erro ao obter métodos de pagamento" });
            }
        }

        /// <summary>
        /// Cria uma preferência de pagamento no Mercado Pago.
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        [HttpPost("pagamento/{paymentId}/cancelar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CancelarPagamento(string paymentId)
        {
            try
            {
                var resultado = await _mercadoPagoService.CancelarPagamentoAsync(paymentId);
                if (resultado)
                    return Ok(new { Message = "Pagamento cancelado com sucesso" });
                else
                    return BadRequest(new { Message = "Não foi possível cancelar o pagamento" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cancelar pagamento {PaymentId}", paymentId);
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Reembolsa um pagamento no Mercado Pago.
        /// </summary>
        /// <param name="paymentId"></param>
        /// <param name="reembolso"></param>
        /// <returns></returns>
        [HttpPost("pagamento/{paymentId}/reembolso")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReembolsarPagamento(string paymentId, [FromBody] ReembolsoDto reembolso)
        {
            try
            {
                RefundResponse resultado;

                if (reembolso.Parcial && reembolso.Valor.HasValue)
                    resultado = await _mercadoPagoService.ProcessarReembolsoParcialAsync(paymentId, reembolso.Valor.Value);
                else
                    resultado = await _mercadoPagoService.ProcessarReembolsoTotalAsync(paymentId);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao reembolsar pagamento {PaymentId}", paymentId);
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Cria um cliente no Mercado Pago.
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        [HttpPost("clientes")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarCliente([FromBody] ClienteDto cliente)
        {
            try
            {
                var resultado = await _mercadoPagoService.CriarClienteAsync(cliente);
                return CreatedAtAction(nameof(ObterCliente), new { clienteId = resultado.Id }, resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar cliente no Mercado Pago");
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Obtém um cliente no Mercado Pago.
        /// </summary>
        /// <param name="clienteId"></param>
        /// <returns></returns>
        [HttpGet("clientes/{clienteId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterCliente(string clienteId)
        {
            try
            {
                var cliente = await _mercadoPagoService.ObterClienteAsync(clienteId);
                return Ok(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter cliente {ClienteId}", clienteId);
                return NotFound(new { Message = "Cliente não encontrado" });
            }
        }

        /// <summary>
        /// Salva um cartão de crédito para um cliente no Mercado Pago.
        /// </summary>
        /// <param name="clienteId"></param>
        /// <param name="cartao"></param>
        /// <returns></returns>
        [HttpPost("clientes/{clienteId}/cartoes")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SalvarCartao(string clienteId, [FromBody] CartaoCreditoDto cartao)
        {
            try
            {
                var cartaoId = await _mercadoPagoService.SalvarCartaoAsync(clienteId, cartao);
                return Created($"/api/mercadopago/clientes/{clienteId}/cartoes/{cartaoId}", new { Id = cartaoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar cartão para cliente {ClienteId}", clienteId);
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Obtém os cartões de crédito salvos para um cliente no Mercado Pago.
        /// </summary>
        /// <param name="clienteId"></param>
        /// <returns></returns>
        [HttpGet("clientes/{clienteId}/cartoes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterCartoes(string clienteId)
        {
            try
            {
                var cartoes = await _mercadoPagoService.ObterCartoesClienteAsync(clienteId);
                return Ok(cartoes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter cartões do cliente {ClienteId}", clienteId);
                return NotFound(new { Message = "Cliente não encontrado ou não possui cartões salvos" });
            }
        }

        /// <summary>
        /// Cria uma preferência de pagamento no Mercado Pago.
        /// </summary>
        /// <param name="preferenceId"></param>
        /// <returns></returns>
        [HttpGet("preferencias/{preferenceId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPreferencia(string preferenceId)
        {
            try
            {
                var preferencia = await _mercadoPagoService.ObterDetalhesPreferenciaAsync(preferenceId);
                return Ok(preferencia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter preferência {PreferenceId}", preferenceId);
                return NotFound(new { Message = "Preferência não encontrada" });
            }
        }

        /// <summary>
        /// Atualiza uma preferência de pagamento no Mercado Pago.
        /// </summary>
        /// <param name="dataInicio"></param>
        /// <param name="dataFim"></param>
        /// <returns></returns>
        [HttpGet("relatorios/transacoes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GerarRelatorioTransacoes([FromQuery] DateTime dataInicio, [FromQuery] DateTime dataFim)
        {
            if (dataInicio > dataFim)
            {
                return BadRequest(new { Message = "Data inicial deve ser anterior à data final" });
            }

            try
            {
                var relatorio = await _mercadoPagoService.GerarRelatorioTransacoesAsync(dataInicio, dataFim);
                return Ok(relatorio);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de transações de {DataInicio} a {DataFim}", dataInicio, dataFim);
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        ///     Valida uma notificação de webhook do Mercado Pago.
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        [HttpPost("webhook/validar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ValidarWebhook([FromBody] WebhookNotificationDto notification)
        {
            try
            {
                var valido = await _mercadoPagoService.ValidarWebhookNotificacaoAsync(notification.Id, notification.Topic);
                return Ok(new { Valido = valido });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar notificação webhook");
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        ///    Obter credenciais de teste para uso em ambiente sandbox.
        /// </summary>
        /// <returns></returns>
        [HttpGet("sandbox/test-credentials")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult ObterCredenciaisTeste()
        {
            return Ok(new
            {
                Message = "Utilize estas credenciais para testes em ambiente sandbox",
                TestUser = new
                {
                    Email = "test_user_123456@testuser.com",
                    CardNumber = "5031 7557 3453 0604",
                    SecurityCode = "123",
                    ExpirationDate = "11/25"
                },
                AvailableCards = new[] {
            new { Type = "Visa", Number = "4235 6477 2802 5682", SecurityCode = "123", ExpirationDate = "11/25" },
            new { Type = "Mastercard", Number = "5031 7557 3453 0604", SecurityCode = "123", ExpirationDate = "11/25" },
            new { Type = "American Express", Number = "3753 651535 56885", SecurityCode = "1234", ExpirationDate = "11/25" }
        }
            });
        }
    }
}
