namespace Web.Domain.DTOs.MercadoPago
{
    public class PaymentMethodDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string PaymentTypeId { get; set; }
        public bool IsDefault { get; set; }
        public string ThumbnailUrl { get; set; }
    }
}
