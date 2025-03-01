using Microsoft.AspNetCore.Identity;
using Web.Domain.Entities;

namespace Web.API.Extensions
{
    public static class SecurityExtensions
    {
        public static bool IsLockedOut(this ApplicationUser user)
        {
            return user.BloqueioExpiraEm.HasValue &&
                   user.BloqueioExpiraEm.Value > DateTime.UtcNow;
        }

        public static async Task<bool> IncrementFailedLoginAttempt(
            this ApplicationUser user,
            UserManager<ApplicationUser> userManager)
        {
            user.TentativasLoginFalhas++;

            // Bloquear após 5 tentativas falhas
            if (user.TentativasLoginFalhas >= 5)
            {
                user.BloqueioExpiraEm = DateTime.UtcNow.AddMinutes(30); // Bloqueio de 30 minutos
            }

            await userManager.UpdateAsync(user);
            return user.IsLockedOut();
        }

        public static async Task ResetLoginAttempts(
            this ApplicationUser user,
            UserManager<ApplicationUser> userManager)
        {
            user.TentativasLoginFalhas = 0;
            user.BloqueioExpiraEm = null;
            await userManager.UpdateAsync(user);
        }
    }
}