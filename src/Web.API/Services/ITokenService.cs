using System.Security.Claims;
using Web.Domain.DTOs;
using Web.Domain.Entities;

namespace Web.API.Services
{
    public interface ITokenService
    {
        Task<TokenResponse> GenerateTokensAsync(ApplicationUser user);

        string GenerateRefreshToken();

        Task<ClaimsPrincipal> ValidateTokenAsync(string token);

        Task<bool> RevokeTokenAsync(string userId);

        Task<UsuarioComRoleENivelAcessoDto> GetUserByIdAsync(string userId);

        Task<IEnumerable<UsuarioComRoleENivelAcessoDto>> SearchUsersAsync(string? nome, string? cpf, string? email, string? telefone);
    }
}