namespace Web.Domain.DTOs
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(string username, string password, string ipAddress, string deviceInfo);

        Task<AuthResult> RefreshTokenAsync(string token, string refreshToken);

        Task<bool> RevokeTokenAsync(string username);

        Task<bool> ValidateTokenAsync(string token);
    }
}