using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Web.Domain.DTOs;
using Web.Domain.Entities;
using Web.Domain.Enums;
using Xunit;

namespace WebApi.Test.Controllers
{
    public class CarrinhoControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;

        public CarrinhoControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _configuration = factory.Services.GetRequiredService<IConfiguration>();
        }

        [Fact]
        public async Task ReduzirQuantidade_ReturnsOk()
        {
            // Arrange
            var token = GenerateJwtToken("4d7a7a18-a8cf-4ffe-b283-481ed450226d", "Cliente"); // Gera um token válido
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Reduzir a quantidade do item
            var reduzirDto = new CarrinhoItemDto
            {
                ProdutoId = 458,
                EstabelecimentoId = 51,
                Quantidade = -1 // Reduzir 1 unidade
            };

            var reduzirContent = new StringContent(JsonConvert.SerializeObject(reduzirDto), Encoding.UTF8, "application/json");
            var reduzirRequest = new HttpRequestMessage(HttpMethod.Put, "/api/carrinho/atualizar-quantidade")
            {
                Content = reduzirContent
            };

            // Act
            var response = await _client.SendAsync(reduzirRequest);

            // Exibir a mensagem de erro (se houver)
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Erro: " + responseContent);
            }

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetCarrinho_ReturnsOk()
        {
            // Arrange
            var token = GenerateJwtToken("4d7a7a18-a8cf-4ffe-b283-481ed450226d", "Cliente"); // Gera um token válido
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new HttpRequestMessage(HttpMethod.Get, "/api/carrinho");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task AdicionarAoCarrinho_ReturnsOk()
        {
            // Arrange
            var token = GenerateJwtToken("4d7a7a18-a8cf-4ffe-b283-481ed450226d", "Cliente"); // Gera um token válido
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var itemDto = new CarrinhoItemDto
            {
                ProdutoId = 458,
                EstabelecimentoId = 51,
                Quantidade = 1,
                ValoresOpcaoProdutoIds = new List<int> { 1, 2 }, // IDs dos valores selecionados
                AdicionalProdutoIds = new List<int> { 3, 4 }, // IDs dos adicionais selecionados
                Observacao = "Sem cebola" // Observações do cliente
            };

            var content = new StringContent(JsonConvert.SerializeObject(itemDto), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/carrinho/adicionar")
            {
                Content = content
            };

            // Act
            var response = await _client.SendAsync(request);

            // Exibir a mensagem de erro (se houver)
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Erro: " + responseContent);
            }

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task AtualizarQuantidade_ReturnsOk()
        {
            // Arrange
            var token = GenerateJwtToken("4d7a7a18-a8cf-4ffe-b283-481ed450226d", "Cliente"); // Gera um token válido
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var itemDto = new CarrinhoItemDto
            {
                ProdutoId = 458,
                EstabelecimentoId = 51,
                Quantidade = 1,
                ValoresOpcaoProdutoIds = new List<int> { 1, 2 }, // IDs dos valores selecionados
                AdicionalProdutoIds = new List<int> { 3, 4 }, // IDs dos adicionais selecionados
                Observacao = "Sem cebola" // Observações do cliente
            };

            var content = new StringContent(JsonConvert.SerializeObject(itemDto), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Put, "/api/carrinho/atualizar-quantidade")
            {
                Content = content
            };

            // Act
            var response = await _client.SendAsync(request);

            // Exibir a mensagem de erro (se houver)
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Erro: " + responseContent);
            }
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task LimparCarrinho_ReturnsOk()
        {
            // Arrange
            var token = GenerateJwtToken("4d7a7a18-a8cf-4ffe-b283-481ed450226d", "Cliente"); // Gera um token válido
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new HttpRequestMessage(HttpMethod.Delete, "/api/carrinho/limpar");

            // Act
            var response = await _client.SendAsync(request);

            // Exibir a mensagem de erro (se houver)
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Erro: " + responseContent);
            }

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private string GenerateJwtToken(string userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, "cliente"), // Nome de usuário
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // ID único do token
                new Claim(JwtRegisteredClaimNames.Email, "cliente@example.com"), // Email
                new Claim("UserId", userId), // ID do usuário
                new Claim("UserName", "cliente"), // Nome de usuário
                new Claim("NomeUsuario", "Cliente Teste"), // Nome completo
                new Claim("EmailConfirmed", "True"), // Email confirmado
                new Claim(ClaimTypes.Role, role) // Role do usuário
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [Fact]
        public async Task AdicionarMultiplosItensAoCarrinho_EstoqueInsuficiente_ReturnsBadRequest()
        {
            // Arrange
            var token = GenerateJwtToken("4d7a7a18-a8cf-4ffe-b283-481ed450226d", "Cliente"); // Gera um token válido
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var itensDto = new List<CarrinhoItemDto>
            {
                new CarrinhoItemDto
                {
                    ProdutoId = 458,
                    EstabelecimentoId = 51,
                    Quantidade = 1000, // Quantidade maior que o estoque
                    ValoresOpcaoProdutoIds = new List<int> { 1, 2 },
                    AdicionalProdutoIds = new List<int> { 3, 4 },
                    Observacao = "Sem cebola"
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(itensDto), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/carrinho/adicionar-multiplos")
            {
                Content = content
            };

            // Act
            var response = await _client.SendAsync(request);

            // Exibir a mensagem de erro (se houver)
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Erro: " + responseContent);
            }

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task AdicionarMultiplosItensAoCarrinho_ReturnsOk()
        {
            // Arrange
            var token = GenerateJwtToken("4d7a7a18-a8cf-4ffe-b283-481ed450226d", "Cliente"); // Gera um token válido
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var itensDto = new List<CarrinhoItemDto>
            {
                new CarrinhoItemDto
                {
                    ProdutoId = 458,
                    EstabelecimentoId = 51,
                    Quantidade = 1,
                    ValoresOpcaoProdutoIds = new List<int> { 1, 2 }, // IDs dos valores selecionados
                    AdicionalProdutoIds = new List<int> { 3, 4 }, // IDs dos adicionais selecionados
                    Observacao = "Sem cebola" // Observações do cliente
                },
                new CarrinhoItemDto
                {
                    ProdutoId = 457, // ProdutoId correto
                    EstabelecimentoId = 51,
                    Quantidade = 1,
                    ValoresOpcaoProdutoIds = new List<int> { 1, 2 }, // IDs dos valores selecionados
                    AdicionalProdutoIds = new List<int> { 3, 4 }, // IDs dos adicionais selecionados
                    Observacao = "Sem cebola" // Observações do cliente
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(itensDto), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/carrinho/adicionar-multiplos")
            {
                Content = content
            };

            // Act
            var response = await _client.SendAsync(request);

            // Exibir a mensagem de erro (se houver)
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Erro: " + responseContent);
            }

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task AdicionarMultiplosItensAoCarrinho_ReturnsOkOne()
        {
            // Arrange
            var token = GenerateJwtToken("4d7a7a18-a8cf-4ffe-b283-481ed450226d", "Cliente"); // Gera um token válido
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var itensDto = new List<CarrinhoItemDto>
            {
                new CarrinhoItemDto
                {
                    ProdutoId = 458,
                    EstabelecimentoId = 51,
                    Quantidade = 1,
                    ValoresOpcaoProdutoIds = new List<int> { 1, 2 }, // IDs dos valores selecionados
                    AdicionalProdutoIds = new List<int> { 3, 4 }, // IDs dos adicionais selecionados
                    Observacao = "Sem cebola" // Observações do cliente
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(itensDto), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/carrinho/adicionar-multiplos")
            {
                Content = content
            };

            // Act
            var response = await _client.SendAsync(request);

            // Exibir a mensagem de erro (se houver)
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Erro: " + responseContent);
            }

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task AtualizarMultiplosItensNoCarrinho_ReturnsOk()
        {
            // Arrange
            var token = GenerateJwtToken("4d7a7a18-a8cf-4ffe-b283-481ed450226d", "Cliente"); // Gera um token válido
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var itens = new List<CarrinhoItem>
            {
                new CarrinhoItem
                {
                    ProdutoId = 458,
                    EstabelecimentoId = 51,
                    Quantidade = 1,
                    Status = StatusCarrinhoItem.Ativo
                },
                new CarrinhoItem
                {
                    ProdutoId = 457,
                    EstabelecimentoId = 51,
                    Quantidade = 1,
                    Status = StatusCarrinhoItem.Ativo
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(itens), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Put, "/api/carrinho/atualizar-multiplos")
            {
                Content = content
            };

            // Act
            var response = await _client.SendAsync(request);

            // Exibir a mensagem de erro (se houver)
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Erro: " + responseContent);
            }

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}