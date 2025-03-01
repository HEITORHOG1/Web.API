namespace Web.Domain.DTOs
{
    public class AuthResult
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public IDictionary<string, string> Claims { get; set; }
    }
}