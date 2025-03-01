using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Web.API.Controllers;
using Web.Application.Interfaces;
using Web.Domain.Entities;
using Web.Domain.Enums;
using Xunit;

namespace WebApi.Test.Controllers
{
    public class CarrinhoControllerTests
    {
        private readonly Mock<ICarrinhoService> _mockCarrinhoService;
        private readonly Mock<IEstabelecimentoService> _mockEstabelecimentoService;
        private readonly Mock<IProdutoService> _mockProdutoService;
        private readonly CarrinhoController _controller;

        public CarrinhoControllerTests()
        {
            _mockCarrinhoService = new Mock<ICarrinhoService>();
            _mockEstabelecimentoService = new Mock<IEstabelecimentoService>();
            _mockProdutoService = new Mock<IProdutoService>();

            _controller = new CarrinhoController(
                _mockCarrinhoService.Object,
                _mockEstabelecimentoService.Object,
                _mockProdutoService.Object
            );

            // Configura o usuário autenticado para os testes
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("UserId", "123")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetCarrinho_ReturnsOk_WithItensAtivos()
        {
            // Arrange
            var userId = "123";
            var itens = new List<CarrinhoItem>
            {
                new CarrinhoItem { ProdutoId = 1, EstabelecimentoId = 1, Quantidade = 2, Status = StatusCarrinhoItem.Ativo },
                new CarrinhoItem { ProdutoId = 2, EstabelecimentoId = 1, Quantidade = 1, Status = StatusCarrinhoItem.Ativo }
            };

            _mockCarrinhoService.Setup(service => service.GetCarrinhoItensAsync(userId))
                .ReturnsAsync(itens);

            // Act
            var result = await _controller.GetCarrinho();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnItens = Assert.IsAssignableFrom<IEnumerable<CarrinhoItem>>(okResult.Value); // Verifica se é IEnumerable<CarrinhoItem>
            Assert.Equal(2, returnItens.Count());
        }
    }
}