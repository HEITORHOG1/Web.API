namespace MarketplaceHybrid.Shared.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<bool> ProcessPaymentAsync(int pedidoId, string paymentUrl);
        Task<PaymentStatus> CheckPaymentStatusAsync(int pedidoId);
        Task<bool> HandlePaymentCallbackAsync(string status, string externalReference);

    }
}
