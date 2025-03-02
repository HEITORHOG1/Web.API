using MercadoPago.Resource.Payment;
using MercadoPago.Resource.Preference;
using Web.Domain.Entities;

namespace Web.API.Services
{
    public interface IMercadoPagoService
    {
        Task<string> CriarPreferenciaAsync(Pedido pedido);
        Task<Payment> ObterPagamentoDoMercadoPagoAsync(string paymentId);
        Task<bool> CancelarPagamentoAsync(string paymentId);
        Task<bool> ReembolsarPagamentoAsync(string paymentId, decimal? valor = null);
        Task<IEnumerable<PaymentMethodDto>> ObterMetodosPagamentoAsync();
        Task<Preference> ObterPreferenciaAsync(string preferenceId);
        Task<string> AtualizarPreferenciaAsync(string preferenceId, Pedido pedido);
        Task<bool> VerificarStatusPagamentoAsync(string paymentId);
        Task<string> RegistrarWebhookAsync(string url);
    }
}