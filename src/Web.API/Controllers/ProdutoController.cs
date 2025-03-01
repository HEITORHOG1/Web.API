using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.API.Services;
using Web.Application.Interfaces;
using Web.Domain.DTOs.Produtos;

namespace Web.API.Controllers
{
    /// <summary>
    /// Controlador de produtos.
    /// </summary>
    [Route("api/produtos")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoService _produtoService;
        private readonly IEstabelecimentoService _estabelecimentoService;
        private readonly IImageUploadService _imageUploadService;
        private readonly IImagemProdutoService _imagemProdutoService;

        /// <summary>
        /// Cria uma nova instância do controlador de produtos.
        /// </summary>
        /// <param name="produtoService"></param>
        /// <param name="estabelecimentoService"></param>
        /// <param name="imageUploadService"></param>
        /// <param name="imagemProdutoService"></param>
        public ProdutosController(IProdutoService produtoService, IEstabelecimentoService estabelecimentoService, IImageUploadService imageUploadService, IImagemProdutoService imagemProdutoService)
        {
            _produtoService = produtoService;
            _estabelecimentoService = estabelecimentoService;
            _imageUploadService = imageUploadService;
            _imagemProdutoService = imagemProdutoService;
        }

        /// <summary>
        /// Cria um novo produto associado a uma categoria e a um estabelecimento.
        /// </summary>
        /// <param name="estabelecimentoId">O ID do estabelecimento.</param>
        /// <param name="produtoDto">Os dados do produto a ser criado.</param>
        /// <returns>Retorna o produto criado com um status de criação (201).</returns>
        [Authorize(Roles = "Proprietario,Gerente")]
        [HttpPost]
        [ProducesResponseType(typeof(ProdutoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create(int estabelecimentoId, [FromForm] ProdutoCreateDto produtoDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Usuário não autenticado");

            try
            {
                var produto = await _produtoService.CreateProdutoAsync(estabelecimentoId, userId, produtoDto);
                return CreatedAtAction(nameof(GetProduto), new { estabelecimentoId, id = produto.Id }, produto);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao criar produto: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém a lista de produtos de um estabelecimento específico.
        /// </summary>
        /// <param name="estabelecimentoId">O ID do estabelecimento.</param>
        /// <returns>Retorna a lista de produtos.</returns>
        [Authorize(Roles = "Proprietario,Gerente")]
        [HttpGet]
        [ProducesResponseType(typeof(List<ProdutoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetProdutos(int estabelecimentoId)
        {
            var userId = User.FindFirstValue("UserId");
            try
            {
                var produtos = await _produtoService.GetProdutosAsync(estabelecimentoId, userId);
                return Ok(produtos);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        /// <summary>
        /// Obtém os detalhes de um produto específico pelo ID.
        /// </summary>
        /// <param name="estabelecimentoId">O ID do estabelecimento.</param>
        /// <param name="id">O ID do produto.</param>
        /// <returns>Retorna os detalhes do produto encontrado ou uma mensagem de erro se não for encontrado.</returns>
        [Authorize(Roles = "Proprietario,Gerente")]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ProdutoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetProduto(int estabelecimentoId, int id)
        {
            var userId = User.FindFirstValue("UserId");
            try
            {
                var produto = await _produtoService.GetProdutoByIdDtoAsync(estabelecimentoId, id, userId);
                return Ok(produto);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Usuário não autenticado");
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Produto não encontrado");
            }
        }

        /// <summary>
        /// Atualiza um produto existente.
        /// </summary>
        /// <param name="estabelecimentoId">O ID do estabelecimento.</param>
        /// <param name="id">O ID do produto a ser atualizado.</param>
        /// <param name="produtoDto">Os novos dados do produto.</param>
        /// <returns>Retorna um status de "NoContent" se a atualização for bem-sucedida.</returns>
        [Authorize(Roles = "Proprietario,Gerente")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutProduto(int estabelecimentoId, int id, [FromForm] ProdutoUpdateDto produtoDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue("UserId");
            try
            {
                await _produtoService.UpdateProdutoAsync(estabelecimentoId, id, userId, produtoDto);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Remove um produto existente pelo ID.
        /// </summary>
        /// <param name="estabelecimentoId">O ID do estabelecimento.</param>
        /// <param name="id">O ID do produto a ser removido.</param>
        /// <returns>Retorna um status de "NoContent" se a remoção for bem-sucedida.</returns>
        [Authorize(Roles = "Proprietario,Gerente")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduto(int estabelecimentoId, int id)
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }

            var estabelecimento = await _estabelecimentoService.GetByIdAsync(estabelecimentoId);
            if (estabelecimento == null || estabelecimento.UsuarioId != userId)
            {
                return Forbid("Você não tem permissão para remover produtos deste estabelecimento.");
            }

            var existingProduto = await _produtoService.GetProdutoByIdAsync(estabelecimentoId, id);
            if (existingProduto == null)
            {
                return NotFound("Produto não encontrado");
            }

            await _produtoService.RemoveProdutoAsync(estabelecimentoId, id);
            return NoContent();
        }

        /// <summary>
        /// Verifica se um produto com o nome especificado já está cadastrado no estabelecimento.
        /// </summary>
        /// <param name="estabelecimentoId">O ID do estabelecimento.</param>
        /// <param name="nomeProduto">O nome do produto a ser verificado.</param>
        /// <returns>Retorna uma lista de nomes de produtos que correspondem ao critério de busca.</returns>
        [Authorize(Roles = "Proprietario,Gerente")]
        [HttpGet("verificar-produto")]
        [ProducesResponseType(typeof(List<ProdutoNomeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> VerificarProdutoCadastrado(int estabelecimentoId, [FromQuery] string nomeProduto)
        {
            if (string.IsNullOrEmpty(nomeProduto))
            {
                return BadRequest("O nome do produto é obrigatório.");
            }

            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }

            var estabelecimento = await _estabelecimentoService.GetByIdAsync(estabelecimentoId);
            if (estabelecimento == null || estabelecimento.UsuarioId != userId)
            {
                return Forbid("Você não tem permissão para verificar produtos neste estabelecimento.");
            }

            var produtos = await _produtoService.ProdutoCadastradoNoEstabelecimentoAsync(estabelecimentoId, nomeProduto);

            if (produtos == null || !produtos.Any())
            {
                return NotFound("Produto não cadastrado.");
            }

            var retorno = produtos.Select(p => new ProdutoNomeDto
            {
                Id = p.Id,
                Nome = p.Nome
            }).ToList();
            return Ok(retorno);
        }
    }
}