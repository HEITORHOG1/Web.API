using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.Application.Interfaces;
using Web.Domain.DTOs.Categorias;

namespace Web.API.Controllers
{
    /// <summary>
    /// Controlador de categorias.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        /// <summary>
        /// Construtor do controlador de categorias.
        /// </summary>
        /// <param name="categoriaService"></param>
        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        /// <summary>
        /// Cria uma nova categoria associada a um estabelecimento.
        /// </summary>
        /// <param name="estabelecimentoId">ID do estabelecimento ao qual a categoria será associada.</param>
        /// <param name="categoriaDto">Os dados da categoria a ser criada.</param>
        /// <returns>Retorna a categoria criada com um status de criação (201).</returns>
        [Authorize(Policy = "ManageCategorias")]
        [HttpPost("estabelecimentos/{estabelecimentoId:int}/categorias")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create(int estabelecimentoId, [FromBody] CategoriaCreateDto categoriaDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = User.FindFirstValue("UserId");
                var categoria = await _categoriaService.CreateCategoriaAsync(estabelecimentoId, userId, categoriaDto);
                return CreatedAtAction(nameof(GetById), new { estabelecimentoId, id = categoria.Id }, categoria);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obtém todas as categorias associadas a um estabelecimento específico.
        /// </summary>
        /// <param name="estabelecimentoId">ID do estabelecimento.</param>
        /// <returns>Retorna uma lista de categorias associadas ao estabelecimento.</returns>
        [Authorize(Policy = "ManageCategorias")]
        [HttpGet("estabelecimentos/{estabelecimentoId:int}/categorias")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllByEstabelecimento(int estabelecimentoId)
        {
            try
            {
                var categorias = await _categoriaService.GetAllByEstabelecimentoIdAsync(estabelecimentoId);
                return Ok(categorias);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Usuário não autenticado");
            }
        }

        /// <summary>
        /// Obtém os detalhes de uma categoria específica pelo ID.
        /// </summary>
        /// <param name="estabelecimentoId">ID do estabelecimento.</param>
        /// <param name="id">O ID da categoria a ser buscada.</param>
        /// <returns>Retorna os detalhes da categoria encontrada ou uma mensagem de erro se não for encontrada.</returns>
        [Authorize(Roles = "Proprietario,Gerente")]
        [HttpGet("estabelecimentos/{estabelecimentoId:int}/categorias/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetById(int estabelecimentoId, int id)
        {
            try
            {
                var categoria = await _categoriaService.GetCategoriaByIdAsync(estabelecimentoId, id);
                return Ok(categoria);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

        }

        /// <summary>
        /// Atualiza uma categoria existente.
        /// </summary>
        /// <param name="estabelecimentoId">ID do estabelecimento.</param>
        /// <param name="id">O ID da categoria a ser atualizada.</param>
        /// <param name="categoriaDto">Os novos dados da categoria.</param>
        /// <returns>Retorna um status de "NoContent" se a atualização for bem-sucedida.</returns>
        [Authorize(Roles = "Proprietario,Gerente")]
        [HttpPut("estabelecimentos/{estabelecimentoId:int}/categorias/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int estabelecimentoId, int id, [FromBody] CategoriaUpdateDto categoriaDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var userId = User.FindFirstValue("UserId");
                await _categoriaService.UpdateCategoriaAsync(estabelecimentoId, id, userId, categoriaDto);
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

        }

        /// <summary>
        /// Remove uma categoria existente pelo ID.
        /// </summary>
        /// <param name="estabelecimentoId">ID do estabelecimento.</param>
        /// <param name="id">O ID da categoria a ser removida.</param>
        /// <returns>Retorna um status de "NoContent" se a remoção for bem-sucedida.</returns>
        [Authorize(Roles = "Proprietario,Gerente")]
        [HttpDelete("estabelecimentos/{estabelecimentoId:int}/categorias/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int estabelecimentoId, int id)
        {
            try
            {
                var userId = User.FindFirstValue("UserId");
                await _categoriaService.DeleteCategoriaAsync(estabelecimentoId, id, userId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}