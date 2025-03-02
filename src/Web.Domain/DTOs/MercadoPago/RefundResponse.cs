namespace Web.Domain.DTOs.MercadoPago
{
    public class RefundResponse
    {
        public string Id { get; set; }
        public string PaymentId { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
