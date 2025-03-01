namespace MarketplaceHybrid.Shared.Models
{
    public class EnderecoClienteResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int? Id { get; set; }
        public string? Logradouro { get; set; }
        public string? Numero { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? CEP { get; set; }
        public bool? Principal { get; set; }
    }
}