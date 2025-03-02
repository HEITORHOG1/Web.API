using MarketplaceHybrid.Shared.Configurations;
using MarketplaceHybrid.Shared.Models;
using MarketplaceHybrid.Shared.Services.Interfaces;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace MarketplaceHybrid.Shared.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;

        public PedidoService(HttpClient httpClient, IAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        public async Task<PedidoCompleto> GetPedidoByIdAsync(int id)
        {
            try
            {
                var token = await GetTokenAsync();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"{ApiConstants.BaseUrl}/Cliente/pedidos/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<PedidoCompleto>();
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter pedido: {ex.Message}");
                return null;
            }
        }

        public async Task<PedidoCompleto> GetPedidoByExternalReferenceAsync(string externalReference)
        {
            try
            {
                var token = await GetTokenAsync();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"{ApiConstants.BaseUrl}/Cliente/pedidos/referencia/{externalReference}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<PedidoCompleto>();
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter pedido por referência externa: {ex.Message}");
                return null;
            }
        }

        public async Task<List<PedidoCompleto>> GetPedidosByUsuarioIdAsync(string userId)
        {
            try
            {
                var token = await GetTokenAsync();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"{ApiConstants.BaseUrl}/Cliente/pedidos");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<PedidoCompleto>>();
                }

                return new List<PedidoCompleto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter pedidos do usuário: {ex.Message}");
                return new List<PedidoCompleto>();
            }
        }

        public async Task<List<PedidoCompleto>> GetPedidosByEstabelecimentoIdAsync(int estabelecimentoId)
        {
            try
            {
                var token = await GetTokenAsync();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"{ApiConstants.BaseUrl}/Estabelecimento/{estabelecimentoId}/pedidos");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<PedidoCompleto>>();
                }

                return new List<PedidoCompleto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter pedidos do estabelecimento: {ex.Message}");
                return new List<PedidoCompleto>();
            }
        }

        public async Task<PedidoCompleto> AddPedidoAsync(Pedido pedido)
        {
            try
            {
                var token = await GetTokenAsync();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PostAsJsonAsync($"{ApiConstants.BaseUrl}/Cliente/finalizar-compra", pedido);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<PedidoCompleto>();
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao adicionar pedido: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdatePedidoAsync(PedidoCompleto pedido)
        {
            try
            {
                var token = await GetTokenAsync();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PutAsJsonAsync($"{ApiConstants.BaseUrl}/Cliente/pedidos/{pedido.Id}", pedido);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar pedido: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> AtualizarStatusPedidoAsync(int pedidoId, StatusPedido novoStatus)
        {
            try
            {
                var token = await GetTokenAsync();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PutAsJsonAsync(
                    $"{ApiConstants.BaseUrl}/Cliente/pedidos/{pedidoId}/status",
                    new { Status = novoStatus });

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar status do pedido: {ex.Message}");
                return false;
            }
        }

        private async Task<string> GetTokenAsync()
        {
            var tokenJson = await _authService.GetTokenAsync();
            if (string.IsNullOrEmpty(tokenJson))
                return string.Empty;

            using var document = JsonDocument.Parse(tokenJson);
            var root = document.RootElement;
            if (root.TryGetProperty("token", out var tokenElement))
            {
                return tokenElement.GetString() ?? string.Empty;
            }

            return string.Empty;
        }
    }
}
