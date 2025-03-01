using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.Application.Interfaces;
using Web.Domain.Entities;

namespace Web.API.Controllers
{
    /// <summary>
    /// Controller para gerenciar os vínculos de usuários com estabelecimentos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioEstabelecimentoController : ControllerBase
    {
        private readonly IUsuarioEstabelecimentoService _usuarioEstabelecimentoService;
        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="usuarioEstabelecimentoService"></param>
        public UsuarioEstabelecimentoController(IUsuarioEstabelecimentoService usuarioEstabelecimentoService)
        {
            _usuarioEstabelecimentoService = usuarioEstabelecimentoService;
        }

        /// <summary>
        /// Obtém todos os vínculos de estabelecimentos associados ao usuário autenticado.
        /// </summary>
        /// <param name="estabelecimentoId">O ID do estabelecimento.</param>
        /// <returns>Uma lista de vínculos de estabelecimentos do usuário autenticado.</returns>
        /// <remarks>
        /// Este método extrai o ID do usuário do token JWT e retorna apenas os vínculos que pertencem a esse usuário.
        /// Caso o ID não seja encontrado, retorna uma resposta de "Não autorizado".
        /// </remarks>
        [Authorize]
        [HttpGet("estabelecimento/{estabelecimentoId:int}")]
        [ProducesResponseType(typeof(IEnumerable<UsuarioEstabelecimento>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllByEstabelecimento(int estabelecimentoId)
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }

            var vinculos = await _usuarioEstabelecimentoService.GetByEstabelecimentoIdAsync(estabelecimentoId);
            if (vinculos == null || !vinculos.Any(v => v.UsuarioId == userId))
            {
                return NotFound("Nenhum vínculo encontrado para este estabelecimento.");
            }

            // Filtrar os vínculos para retornar apenas os que pertencem ao usuário autenticado
            var vinculosDoUsuario = vinculos.Where(v => v.UsuarioId == userId).ToList();

            return Ok(vinculosDoUsuario);
        }

        /// <summary>
        /// Obtém os detalhes de um vínculo específico pelo ID.
        /// </summary>
        /// <param name="id">O ID do vínculo a ser buscado.</param>
        /// <returns>Os detalhes do vínculo encontrado ou uma mensagem de erro se não for encontrado ou não autorizado.</returns>
        /// <remarks>
        /// Este método verifica se o vínculo existe e se o usuário autenticado é o proprietário do vínculo.
        /// Retorna uma resposta de "Não encontrado" se o vínculo não existir ou não pertencer ao usuário.
        /// </remarks>
        [Authorize]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(UsuarioEstabelecimento), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }

            var vinculo = await _usuarioEstabelecimentoService.GetByIdAsync(id);

            if (vinculo == null)
            {
                return NotFound("Vínculo não encontrado");
            }

            if (vinculo.UsuarioId != userId)
            {
                return Forbid("Você não tem permissão para acessar este vínculo.");
            }

            return Ok(vinculo);
        }
    }
}