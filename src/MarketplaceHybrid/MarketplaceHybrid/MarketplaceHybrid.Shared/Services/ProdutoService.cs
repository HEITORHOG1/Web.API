using MarketplaceHybrid.Shared.Configurations;
using MarketplaceHybrid.Shared.Models;
using MarketplaceHybrid.Shared.Services.Interfaces;
using System.Net.Http.Json;

namespace MarketplaceHybrid.Shared.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly HttpClient _httpClient;

        public ProdutoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Cardapio>> GetProdutosByEstabelecimentoIdAsync(int estabelecimentoId)
        {
            try
            {
                // Use a constante para o endpoint
                var response = await _httpClient.GetFromJsonAsync<List<ProdutoResponse>>(
                    $"{Endpoints.GetProdutosByEstabelecimentoId}?Id={estabelecimentoId}");

                return response?.FirstOrDefault()?.Cardapio ?? new List<Cardapio>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar produtos: {ex.Message}");
                return new List<Cardapio>();
            }
        }
        public async Task<Cardapio?> GetProdutoByIdAsync(int estabelecimentoId, int produtoId)
        {
            try
            {
                // Usando string.Format para preencher os parâmetros do endpoint
                var url = string.Format(Endpoints.GetProdutoById, estabelecimentoId, produtoId);
                var response = await _httpClient.GetFromJsonAsync<Cardapio>(url);
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar produto: {ex.Message}");
                return null;
            }
        }

    }
}