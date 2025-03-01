using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Web.API.Services;
using Web.Domain.DTOs;
using Web.Domain.Entities;
using Web.Domain.Enums;
using Web.Infrastructure.Data.Context;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<TokenService> _logger;
    private readonly AppDbContext _context;

    public TokenService(
        IConfiguration configuration,
        UserManager<ApplicationUser> userManager,
        ILogger<TokenService> logger, AppDbContext context)
    {
        _configuration = configuration;
        _userManager = userManager;
        _logger = logger;
        _context = context;
    }

    public async Task<TokenResponse> GenerateTokensAsync(ApplicationUser user)
    {
        try
        {
            var claims = await GenerateUserClaimsAsync(user);
            var token = GenerateJwtToken(claims);
            var refreshToken = GenerateRefreshToken();
            var roles = await _userManager.GetRolesAsync(user);

            // Atualiza os dados de login do usuário
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            user.UltimoLogin = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            // Agrupar claims por tipo
            var claimsDict = claims.GroupBy(c => c.Type)
                                  .ToDictionary(g => g.Key, g => g.Select(c => c.Value));

            return new TokenResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])),
                Roles = roles,
                Claims = claimsDict
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar tokens para usuário {UserId}", user.Id);
            throw;
        }
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public async Task<ClaimsPrincipal> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Token inválido");
            }

            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar token");
            throw;
        }
    }

    public async Task<bool> RevokeTokenAsync(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao revogar token para usuário {UserId}", userId);
            throw;
        }
    }

    private async Task<List<Claim>> GenerateUserClaimsAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("UserId", user.Id), // Este claim é necessário para validar o token
            new("UserName", user.UserName),
            new("NomeUsuario", user.NomeUsuario ?? ""),
            new("EmailConfirmed", user.EmailConfirmed.ToString())
        };

        // Adicionar roles
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Adicionar permissões
        var permissions = new HashSet<string>();
        foreach (var role in roles)
        {
            if (UserRoles.Permissions.TryGetValue(role, out var rolePermissions))
            {
                foreach (var permission in rolePermissions)
                {
                    permissions.Add(permission);
                }
            }
        }

        claims.AddRange(permissions.Select(p => new Claim("permission", p)));

        return claims;
    }

    private string GenerateJwtToken(IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<UsuarioComRoleENivelAcessoDto> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return null;
        }

        var role = await _userManager.GetRolesAsync(user);
        var nivelAcesso = await _context.UsuariosEstabelecimentos
            .Where(ue => ue.UsuarioId == userId)
            .Select(ue => ue.NivelAcesso)
            .FirstOrDefaultAsync();

        return new UsuarioComRoleENivelAcessoDto
        {
            Id = user.Id,
            NomeUsuario = user.NomeUsuario,
            Email = user.Email,
            NivelAcesso = nivelAcesso,
            CPF_CNPJ = user.CPF_CNPJ,
            Cep = user.CEP,
            UserName = user.UserName,
            Endereco = user.Endereco,
            Telefone = user.Telefone,
            Ativo = user.Ativo,
            Role = role.FirstOrDefault()
        };
    }

    public async Task<IEnumerable<UsuarioComRoleENivelAcessoDto>> SearchUsersAsync(string? nome, string? cpf, string? email, string? telefone)
    {
        var query = _userManager.Users.AsQueryable();

        if (!string.IsNullOrEmpty(nome))
        {
            query = query.Where(u => u.NomeUsuario.Contains(nome));
        }

        if (!string.IsNullOrEmpty(cpf))
        {
            query = query.Where(u => u.CPF_CNPJ.Contains(cpf));
        }

        if (!string.IsNullOrEmpty(email))
        {
            query = query.Where(u => u.Email.Contains(email));
        }

        if (!string.IsNullOrEmpty(telefone))
        {
            query = query.Where(u => u.Telefone.Contains(telefone));
        }

        var users = await query.ToListAsync();

        var userDtos = new List<UsuarioComRoleENivelAcessoDto>();

        foreach (var user in users)
        {
            var role = await _userManager.GetRolesAsync(user);
            var nivelAcesso = await _context.UsuariosEstabelecimentos
                .Where(ue => ue.UsuarioId == user.Id)
                .Select(ue => ue.NivelAcesso)
                .FirstOrDefaultAsync();

            userDtos.Add(new UsuarioComRoleENivelAcessoDto
            {
                Id = user.Id,
                NomeUsuario = user.NomeUsuario,
                Email = user.Email,
                NivelAcesso = nivelAcesso,
                CPF_CNPJ = user.CPF_CNPJ,
                Cep = user.CEP,
                UserName = user.UserName,
                Endereco = user.Endereco,
                Telefone = user.Telefone,
                Ativo = user.Ativo,
                Role = role.FirstOrDefault()
            });
        }

        return userDtos;
    }
}