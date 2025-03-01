namespace MarketplaceHybrid.Shared.Models
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public List<string> Roles { get; set; }
        public UserResponse User { get; set; }
    }
}