using MercadoPago.Resource.Payment;
using MercadoPago.Resource.Preference;
using Web.Domain.DTOs.MercadoPago;
using Web.Domain.Entities;

namespace Web.API.Services
{
    public interface IMercadoPagoService
    {
        // Métodos existentes
        Task<string> CriarPreferenciaAsync(Pedido pedido);
        Task<Payment> ObterPagamentoDoMercadoPagoAsync(string paymentId);
        Task<bool> CancelarPagamentoAsync(string paymentId);
        Task<bool> ReembolsarPagamentoAsync(string paymentId, decimal? valor = null);
        Task<IEnumerable<PaymentMethodDto>> ObterMetodosPagamentoAsync();
        Task<Preference> ObterPreferenciaAsync(string preferenceId);
        Task<string> AtualizarPreferenciaAsync(string preferenceId, Pedido pedido);
        Task<bool> VerificarStatusPagamentoAsync(string paymentId);

        // Novos métodos
        Task<string> RegistrarWebhookAsync(string url);
        Task<bool> ValidarWebhookNotificacaoAsync(string notificationId, string topic);
        Task<CustomerDto> CriarClienteAsync(ClienteDto cliente);
        Task<CustomerDto> ObterClienteAsync(string clienteId);
        Task<string> SalvarCartaoAsync(string clienteId, CartaoCreditoDto cartao);
        Task<IEnumerable<CartaoSalvoDto>> ObterCartoesClienteAsync(string clienteId);
        Task<IEnumerable<PaymentMethodDto>> ObterMetodosPagamentoPorPaisAsync(string countryId);
        Task<RefundResponse> ProcessarReembolsoTotalAsync(string paymentId);
        Task<RefundResponse> ProcessarReembolsoParcialAsync(string paymentId, decimal valor);
        Task<PreferenceDetailsDto> ObterDetalhesPreferenciaAsync(string preferenceId);
        Task<TransactionReportDto> GerarRelatorioTransacoesAsync(DateTime dataInicio, DateTime dataFim);
    }
}