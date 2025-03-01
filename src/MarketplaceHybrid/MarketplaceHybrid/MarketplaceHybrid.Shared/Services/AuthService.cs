using MarketplaceHybrid.Shared.Configurations;
using MarketplaceHybrid.Shared.Models;
using MarketplaceHybrid.Shared.Services.Interfaces;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace MarketplaceHybrid.Shared.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;
        private const string TokenKey = "auth_token";
        private const string UsernameKey = "auth_username";

        // Armazena valores em memória até o JavaScript Interop estar disponível
        private string? _tokenInMemory;
        private string? _usernameInMemory;

        public AuthService(HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync(Endpoints.AuthLogin, new
            {
                username,
                password
            });

            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                _tokenInMemory = token;
                _usernameInMemory = username;
                await SaveTokenAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> RegisterAsync(UserRegistrationModel model)
        {
            var response = await _httpClient.PostAsJsonAsync(Endpoints.AuthRegister, model);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CheckUsernameExistsAsync(string username)
        {
            var response = await _httpClient.GetAsync($"{Endpoints.CheckUsernameExists}?username={username}");
            return response.IsSuccessStatusCode && await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> IsLoggedInAsync()
        {
            if (!string.IsNullOrEmpty(_tokenInMemory))
                return true;

            _tokenInMemory = await GetFromLocalStorageAsync(TokenKey);
            return !string.IsNullOrEmpty(_tokenInMemory);
        }

        public async Task<string> GetLoggedInUsernameAsync()
        {
            if (!string.IsNullOrEmpty(_usernameInMemory))
                return _usernameInMemory;

            _usernameInMemory = await GetFromLocalStorageAsync(UsernameKey);
            return _usernameInMemory ?? string.Empty;
        }

        public async Task<string> GetTokenAsync()
        {
            if (!string.IsNullOrEmpty(_tokenInMemory))
                return _tokenInMemory;

            _tokenInMemory = await GetFromLocalStorageAsync(TokenKey);
            return _tokenInMemory ?? string.Empty;
        }

        public async Task LogoutAsync()
        {
            _tokenInMemory = null;
            _usernameInMemory = null;

            if (_jsRuntime != null)
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", UsernameKey);
            }
        }

        private async Task SaveTokenAsync()
        {
            if (_jsRuntime != null)
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, _tokenInMemory);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", UsernameKey, _usernameInMemory);
            }
        }

        private async Task<string?> GetFromLocalStorageAsync(string key)
        {
            try
            {
                return _jsRuntime != null
                    ? await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key)
                    : null;
            }
            catch
            {
                return null;
            }
        }
    }
}
