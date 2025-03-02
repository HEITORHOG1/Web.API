using MarketplaceHybrid.Shared.Configurations;
using MarketplaceHybrid.Shared.Models;
using MarketplaceHybrid.Shared.Services.Interfaces;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace MarketplaceHybrid.Shared.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private readonly IJSRuntime _jsRuntime;
        private readonly ILocalStorageService _localStorageService;

        public PaymentService(
            HttpClient httpClient,
            IAuthService authService,
            IJSRuntime jsRuntime,
            ILocalStorageService localStorageService)
        {
            _httpClient = httpClient;
            _authService = authService;
            _jsRuntime = jsRuntime;
            _localStorageService = localStorageService;
        }

        public async Task<bool> ProcessPaymentAsync(int pedidoId, string paymentUrl)
        {
            try
            {
                // Store the current order ID for callback handling
                await _localStorageService.SetItemAsync("currentOrderId", pedidoId);

                // Redirect to payment URL
                await _jsRuntime.InvokeVoidAsync("window.location.replace", paymentUrl);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar pagamento: {ex.Message}");
                return false;
            }
        }

        public async Task<PaymentStatus> CheckPaymentStatusAsync(int pedidoId)
        {
            try
            {
                var token = await GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    return PaymentStatus.Unknown;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"{ApiConstants.BaseUrl}/Cliente/pedidos/{pedidoId}");

                if (response.IsSuccessStatusCode)
                {
                    var pedido = await response.Content.ReadFromJsonAsync<PedidoCompleto>();

                    return pedido?.Status switch
                    {
                        StatusPedido.PagamentoAprovado => PaymentStatus.Approved,
                        StatusPedido.AguardandoPagamento => PaymentStatus.Pending,
                        StatusPedido.Cancelado => PaymentStatus.Rejected,
                        _ => PaymentStatus.Unknown
                    };
                }

                return PaymentStatus.Unknown;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao verificar status do pagamento: {ex.Message}");
                return PaymentStatus.Unknown;
            }
        }

        public async Task<bool> HandlePaymentCallbackAsync(string status, string externalReference)
        {
            try
            {
                // Get the current order ID from storage
                var pedidoId = await _localStorageService.GetItemAsync<int>("currentOrderId");

                if (pedidoId <= 0)
                {
                    // Try to get the order by external reference if order ID is not available
                    var token1 = await GetTokenAsync();
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token1);

                    var response1 = await _httpClient.GetAsync($"{ApiConstants.BaseUrl}/Cliente/pedidos/referencia/{externalReference}");

                    if (response1.IsSuccessStatusCode)
                    {
                        var pedido = await response1.Content.ReadFromJsonAsync<PedidoCompleto>();
                        pedidoId = pedido?.Id ?? 0;
                    }
                }

                if (pedidoId <= 0)
                {
                    return false;
                }

                // Clear the current order ID
                await _localStorageService.RemoveItemAsync("currentOrderId");

                // Update the order status based on the payment status
                var novoStatus = status.ToLower() switch
                {
                    "approved" => StatusPedido.PagamentoAprovado,
                    "pending" => StatusPedido.AguardandoPagamento,
                    "rejected" or "failure" => StatusPedido.Cancelado,
                    _ => StatusPedido.AguardandoPagamento
                };

                var token = await GetTokenAsync();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PutAsJsonAsync(
                    $"{ApiConstants.BaseUrl}/Cliente/pedidos/{pedidoId}/status",
                    new { Status = novoStatus });

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar retorno do pagamento: {ex.Message}");
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

    public enum PaymentStatus
    {
        Pending,
        Approved,
        Rejected,
        Unknown
    }
}
