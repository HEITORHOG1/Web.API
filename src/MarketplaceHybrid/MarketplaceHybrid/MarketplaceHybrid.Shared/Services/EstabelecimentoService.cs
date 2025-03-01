using MarketplaceHybrid.Shared.Configurations;
using MarketplaceHybrid.Shared.Models;
using MarketplaceHybrid.Shared.Services.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace MarketplaceHybrid.Shared.Services
{
    public class EstabelecimentoService : IEstabelecimentoService
    {
        private readonly HttpClient _httpClient;

        public EstabelecimentoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<EstabelecimentoResponse?> GetEstabelecimentoByIdAsync(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<EstabelecimentoResponse>(
                $"{Endpoints.GetEstabelecimentoById}?Id={id}");
            return response;
        }

        public async Task<EnderecoDto> BuscarEnderecoPorCepAsync(string cep)
        {
            var url = $"https://viacep.com.br/ws/{cep}/json/";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<EnderecoDto>(content);
            }

            return null;
        }
    }
}