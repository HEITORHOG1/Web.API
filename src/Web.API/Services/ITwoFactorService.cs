using Web.Domain.Entities;

namespace Web.API.Services
{
    public interface ITwoFactorService
    {
        Task<bool> GenerateTwoFactorTokenAsync(ApplicationUser user);

        Task<bool> ValidateTwoFactorTokenAsync(ApplicationUser user, string token);

        Task<bool> EnableTwoFactorAsync(ApplicationUser user);

        Task<bool> DisableTwoFactorAsync(ApplicationUser user);
    }
}