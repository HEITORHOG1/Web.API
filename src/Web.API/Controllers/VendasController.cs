using Microsoft.AspNetCore.Mvc;
using Web.Application.Interfaces;
using Web.Domain.DTOs.Produtos;
using Web.Domain.Paginacao;

namespace Web.API.Controllers
{
    /// <summary>
    /// Controller para operações de vendas.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class VendasController : ControllerBase
    {
        private readonly IProdutoService _produtoService;
        private readonly IEstabelecimentoService _estabelecimentoService;
        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="produtoService"></param>
        /// <param name="estabelecimentoService"></param>
        public VendasController(IProdutoService produtoService, IEstabelecimentoService estabelecimentoService)
        {
            _produtoService = produtoService;
            _estabelecimentoService = estabelecimentoService;
        }

        /// <summary>
        /// Obtém uma lista paginada de produtos de um estabelecimento específico para loja online.
        /// </summary>
        /// <param name="estabelecimentoId">O ID do estabelecimento.</param>
        /// <param name="pageNumber">O número da página.</param>
        /// <param name="pageSize">O tamanho da página.</param>
        /// <returns>Retorna a lista paginada de produtos.</returns>
        [HttpGet("estabelecimentos/{estabelecimentoId:int}/produtos")]
        [ProducesResponseType(typeof(PagedResult<ProdutoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProdutos(int estabelecimentoId)
        {
            // Verificar se o estabelecimento existe
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(estabelecimentoId);
            if (estabelecimento == null)
            {
                return NotFound("Estabelecimento não encontrado");
            }

            var produtosPaginados = await _produtoService.GetProdutosByEstabelecimentoIdAsync(estabelecimentoId);

            return Ok(produtosPaginados);
        }
    }
}