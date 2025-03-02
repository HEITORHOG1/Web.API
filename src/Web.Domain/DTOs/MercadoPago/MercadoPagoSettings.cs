namespace Web.Domain.DTOs.MercadoPago
{
    public class MercadoPagoSettings
    {
        public string AccessToken { get; set; }
        public string PublicKey { get; set; }
        public string ClientSecret { get; set; }
        public bool UseSandbox { get; set; } = true;
        public int TimeoutSeconds { get; set; } = 30;
    }
}