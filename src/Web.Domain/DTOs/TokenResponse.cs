namespace Web.Domain.DTOs
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public IDictionary<string, IEnumerable<string>> Claims { get; set; } // Modificado para suportar múltiplos valores
    }
}