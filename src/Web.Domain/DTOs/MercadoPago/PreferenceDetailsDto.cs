namespace Web.Domain.DTOs.MercadoPago
{
    public class PreferenceDetailsDto
    {
        public string Id { get; set; }
        public string InitPoint { get; set; }
        public string SandboxInitPoint { get; set; }
        public string ExternalReference { get; set; }
        public List<PreferenceItemDto> Items { get; set; }
        public PreferencePayerDto Payer { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? ExpirationDateFrom { get; set; }
        public DateTime? ExpirationDateTo { get; set; }
    }
}
