namespace Web.Domain.DTOs.MercadoPago
{
    public class CartaoSalvoDto
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string LastFourDigits { get; set; }
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
        public string PaymentMethodId { get; set; }
        public string PaymentTypeId { get; set; }
        public string CardholderName { get; set; }
        public string SecurityCode { get; set; }
        public string Issuer { get; set; }
    }
}
