using MarketplaceHybrid.Shared.Models;

public interface IAuthService
{
    Task<bool> LoginAsync(string username, string password);
    Task<bool> RegisterAsync(UserRegistrationModel model);
    Task<bool> CheckUsernameExistsAsync(string username);
    Task<bool> IsLoggedInAsync(); // Correto
    Task<string> GetLoggedInUsernameAsync();
    Task<string> GetTokenAsync();
    Task LogoutAsync();
}
