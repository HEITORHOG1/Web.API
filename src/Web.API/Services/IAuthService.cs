using Web.Domain.DTOs;

namespace Web.API.Services
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(string username, string password, string ipAddress, string deviceInfo);

        Task<AuthResult> RefreshTokenAsync(string token, string refreshToken);

        Task<bool> RevokeTokenAsync(string username);

        Task<bool> ValidateTokenAsync(string token);
    }
}