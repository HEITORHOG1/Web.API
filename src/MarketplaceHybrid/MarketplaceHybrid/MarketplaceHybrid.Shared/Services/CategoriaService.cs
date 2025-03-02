using MarketplaceHybrid.Shared.Configurations;
using MarketplaceHybrid.Shared.Models;
using MarketplaceHybrid.Shared.Services.Interfaces;
using System.Net.Http.Json;

namespace MarketplaceHybrid.Shared.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly HttpClient _httpClient;

        public CategoriaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Categoria>> GetCategoriasByEstabelecimentoIdAsync(int estabelecimentoId)
        {
            try
            {
                var url = $"{ApiConstants.BaseUrl}/Categorias/estabelecimentos/{estabelecimentoId}/categorias";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var categorias = await response.Content.ReadFromJsonAsync<List<Categoria>>();
                    return categorias ?? new List<Categoria>();
                }

                // Se não conseguir obter da API, tenta retornar categorias padrão
                return GetDefaultCategorias();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter categorias: {ex.Message}");
                return GetDefaultCategorias();
            }
        }

        public async Task<Categoria> GetCategoriaByIdAsync(int categoriaId)
        {
            try
            {
                var url = $"{ApiConstants.BaseUrl}/Categorias/{categoriaId}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var categoria = await response.Content.ReadFromJsonAsync<Categoria>();
                    return categoria;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter categoria: {ex.Message}");
                return null;
            }
        }

        // Método para retornar categorias padrão caso a API falhe
        private List<Categoria> GetDefaultCategorias()
        {
            return new List<Categoria>
            {
                new Categoria { Id = 1, Nome = "Lanches", Descricao = "Hambúrgueres e sanduíches", Ordem = 1 },
                new Categoria { Id = 2, Nome = "Pizzas", Descricao = "Pizzas salgadas e doces", Ordem = 2 },
                new Categoria { Id = 3, Nome = "Bebidas", Descricao = "Refrigerantes, sucos e cervejas", Ordem = 3 },
                new Categoria { Id = 4, Nome = "Sobremesas", Descricao = "Sobremesas e doces", Ordem = 4 },
                new Categoria { Id = 5, Nome = "Combos", Descricao = "Combos promocionais", Ordem = 5 }
            };
        }
    }
}
