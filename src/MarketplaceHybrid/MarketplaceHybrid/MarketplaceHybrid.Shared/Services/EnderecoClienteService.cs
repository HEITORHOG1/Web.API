// MarketplaceHybrid.Shared/Services/EnderecoClienteService.cs
using MarketplaceHybrid.Shared.Configurations;
using MarketplaceHybrid.Shared.Models;
using MarketplaceHybrid.Shared.Services.Interfaces;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace MarketplaceHybrid.Shared.Services
{
    public class EnderecoClienteService : IEnderecoClienteService
    {
        private readonly HttpClient _httpClient;

        public EnderecoClienteService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<EnderecoClienteResponse?> AddEnderecoAsync(EnderecoClienteDto enderecoDto, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsJsonAsync($"{Endpoints.AddEnderecoCliente}", enderecoDto);
            try
            {
                response.EnsureSuccessStatusCode();
                var endereco = await response.Content.ReadFromJsonAsync<EnderecoClienteDto>();
                return new EnderecoClienteResponse
                {
                    Success = true,
                    Id = endereco.Id,
                    Bairro = endereco.Bairro,
                    CEP = endereco.CEP,
                    Cidade = endereco.Cidade,
                    Complemento = endereco.Complemento,
                    Estado = endereco.Estado,
                    Logradouro = endereco.Logradouro,
                    Numero = endereco.Numero,
                    Principal = endereco.Principal
                };
            }
            catch (Exception ex)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Erro ao adicionar endereço: {ex.Message}");
                var enderecoResponse = new EnderecoClienteResponse
                {
                    Success = false,
                    Message = $"Erro ao adicionar endereço: {error}",
                };
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<EnderecoClienteResponse>(error, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (errorResponse != null)
                    {
                        enderecoResponse.Message = errorResponse.Message;
                        enderecoResponse.Id = errorResponse.Id;
                        enderecoResponse.CEP = errorResponse.CEP;
                        enderecoResponse.Bairro = errorResponse.Bairro;
                        enderecoResponse.Cidade = errorResponse.Cidade;
                        enderecoResponse.Complemento = errorResponse.Complemento;
                        enderecoResponse.Estado = errorResponse.Estado;
                        enderecoResponse.Logradouro = errorResponse.Logradouro;
                        enderecoResponse.Numero = errorResponse.Numero;
                        enderecoResponse.Principal = errorResponse.Principal;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Erro ao deserializar erro: {e.Message}");
                }
                return enderecoResponse;
            }
        }
        public async Task<IEnumerable<EnderecoClienteDto>?> GetAllByUsuarioIdAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"{Endpoints.GetEnderecoCliente}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<EnderecoClienteDto>>();
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Erro ao buscar endereços: {error}");
                return null;
            }
        }

        public async Task<EnderecoClienteDto?> GetPrincipalByUsuarioIdAsync(string userId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userId);
            var response = await _httpClient.GetAsync($"{Endpoints.GetEnderecoCliente}/principal");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<EnderecoClienteDto>();
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Erro ao buscar endereço principal: {error}");
                return null;
            }
        }

        public async Task<EnderecoClienteResponse?> UpdateEnderecoAsync(EnderecoClienteDto enderecoDto, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PutAsJsonAsync($"{Endpoints.AddEnderecoCliente}/{enderecoDto.Id}", enderecoDto);
            try
            {
                response.EnsureSuccessStatusCode();
                var endereco = await response.Content.ReadFromJsonAsync<EnderecoClienteDto>();
                return new EnderecoClienteResponse
                {
                    Success = true,
                    Id = endereco.Id,
                    Bairro = endereco.Bairro,
                    CEP = endereco.CEP,
                    Cidade = endereco.Cidade,
                    Complemento = endereco.Complemento,
                    Estado = endereco.Estado,
                    Logradouro = endereco.Logradouro,
                    Numero = endereco.Numero,
                    Principal = endereco.Principal
                };
            }
            catch (Exception ex)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Erro ao atualizar endereço: {ex.Message}");
                var enderecoResponse = new EnderecoClienteResponse
                {
                    Success = false,
                    Message = $"Erro ao atualizar endereço: {error}",
                };
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<EnderecoClienteResponse>(error, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (errorResponse != null)
                    {
                        enderecoResponse.Message = errorResponse.Message;
                        enderecoResponse.Id = errorResponse.Id;
                        enderecoResponse.CEP = errorResponse.CEP;
                        enderecoResponse.Bairro = errorResponse.Bairro;
                        enderecoResponse.Cidade = errorResponse.Cidade;
                        enderecoResponse.Complemento = errorResponse.Complemento;
                        enderecoResponse.Estado = errorResponse.Estado;
                        enderecoResponse.Logradouro = errorResponse.Logradouro;
                        enderecoResponse.Numero = errorResponse.Numero;
                        enderecoResponse.Principal = errorResponse.Principal;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Erro ao deserializar erro: {e.Message}");
                }
                return enderecoResponse;
            }
        }
    }
}
