using MercadoPago.Resource.Payment;
using Web.Domain.Entities;

namespace Web.API.Services
{
    public interface IMercadoPagoService
    {
        Task<string> CriarPreferenciaAsync(Pedido pedido);

        Task<Payment> ObterPagamentoDoMercadoPagoAsync(string paymentId);
    }
}