using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.Application.Interfaces;
using Web.Domain.DTOs.Fornecedor;
using Web.Domain.Entities;

namespace Web.API.Controllers
{
    /// <summary>
    /// Controller para gerenciar fornecedores.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FornecedoresController : ControllerBase
    {
        private readonly IFornecedorService _fornecedorService;
        private readonly IEstabelecimentoService _estabelecimentoService;
        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="fornecedorService"></param>
        /// <param name="estabelecimentoService"></param>
        public FornecedoresController(IFornecedorService fornecedorService, IEstabelecimentoService estabelecimentoService)
        {
            _fornecedorService = fornecedorService;
            _estabelecimentoService = estabelecimentoService;
        }

        /// <summary>
        /// Obtém todos os fornecedores associados a um estabelecimento específico.
        /// </summary>
        /// <param name="estabelecimentoId">ID do estabelecimento.</param>
        /// <returns>Uma lista de fornecedores do estabelecimento.</returns>
        [Authorize(Policy = "ViewFornecedores")]
        [HttpGet("estabelecimentos/{estabelecimentoId:int}")]
        [ProducesResponseType(typeof(IEnumerable<Fornecedor>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllByEstabelecimento(int estabelecimentoId)
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }

            // Verificar se o usuário tem permissão para visualizar fornecedores neste estabelecimento
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(estabelecimentoId);
            if (estabelecimento == null || estabelecimento.UsuarioId != userId)
            {
                return Forbid("Você não tem permissão para visualizar fornecedores deste estabelecimento.");
            }

            var fornecedores = await _fornecedorService.GetAllByEstabelecimentoIdAsync(estabelecimentoId);
            return Ok(fornecedores);
        }

        /// <summary>
        /// Obtém os detalhes de um fornecedor específico pelo ID.
        /// </summary>
        /// <param name="id">O ID do fornecedor.</param>
        /// <returns>Os detalhes do fornecedor encontrado ou uma mensagem de erro se não for encontrado.</returns>
        [Authorize(Policy = "ViewFornecedores")]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Fornecedor), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var fornecedor = await _fornecedorService.GetByIdAsync(id);
            if (fornecedor == null)
            {
                return NotFound("Fornecedor não encontrado");
            }

            return Ok(fornecedor);
        }

        /// <summary>
        /// Cria um novo fornecedor associado a um estabelecimento.
        /// </summary>
        /// <param name="estabelecimentoId">ID do estabelecimento.</param>
        /// <param name="fornecedorDto">Os dados do fornecedor a ser criado.</param>
        /// <returns>Retorna o fornecedor criado com um status de criação (201).</returns>
        [Authorize(Policy = "ManageFornecedores")]
        [HttpPost("estabelecimentos/{estabelecimentoId:int}")]
        [ProducesResponseType(typeof(Fornecedor), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(int estabelecimentoId, [FromBody] FornecedorCreateDto fornecedorDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //verificar o ExistsAsync
            var fornecedorExists = await _fornecedorService.ExistsAsync(fornecedorDto.EstabelecimentoId, fornecedorDto.Nome);
            if (fornecedorExists)
            {
                return BadRequest("Já existe um fornecedor com este Nome.");
            }
            //verificar o ExistsAsync
            var fornecedorCnpjExists = await _fornecedorService.ExistsAsyncCNPJ(fornecedorDto.EstabelecimentoId, fornecedorDto.CNPJ);
            if (fornecedorCnpjExists)
            {
                return BadRequest("Já existe um fornecedor com este Cnpj.");
            }

            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }

            // Verificar se o usuário tem permissão para criar fornecedores neste estabelecimento
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(estabelecimentoId);
            if (estabelecimento == null || estabelecimento.UsuarioId != userId)
            {
                return Forbid("Você não tem permissão para adicionar fornecedores neste estabelecimento.");
            }

            var fornecedor = new Fornecedor
            {
                Nome = fornecedorDto.Nome,
                CNPJ = fornecedorDto.CNPJ,
                Telefone = fornecedorDto.Telefone,
                Endereco = fornecedorDto.Endereco,
                Email = fornecedorDto.Email,
                EstabelecimentoId = estabelecimentoId // Associar o fornecedor ao estabelecimento
            };

            await _fornecedorService.AddFornecedorAsync(fornecedor);

            return CreatedAtAction(nameof(GetById), new { id = fornecedor.Id }, fornecedor);
        }

        /// <summary>
        /// Atualiza um fornecedor existente.
        /// </summary>
        /// <param name="id">O ID do fornecedor a ser atualizado.</param>
        /// <param name="fornecedorDto">Os novos dados do fornecedor.</param>
        /// <returns>Retorna um status de "NoContent" se a atualização for bem-sucedida.</returns>
        [Authorize(Policy = "ManageFornecedores")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] FornecedorUpdateDto fornecedorDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }

            var existingFornecedor = await _fornecedorService.GetByIdAsync(id);
            if (existingFornecedor == null)
            {
                return NotFound("Fornecedor não encontrado");
            }

            // Verificar se o fornecedor pertence ao estabelecimento do usuário
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(existingFornecedor.EstabelecimentoId);
            if (estabelecimento == null || estabelecimento.UsuarioId != userId)
            {
                return Forbid("Você não tem permissão para atualizar este fornecedor.");
            }

            // Garantir que o fornecedor continue associado ao mesmo estabelecimento
            if (existingFornecedor.EstabelecimentoId != fornecedorDto.EstabelecimentoId)
            {
                return BadRequest("Não é possível alterar o estabelecimento associado a um fornecedor.");
            }

            existingFornecedor.Nome = fornecedorDto.Nome;
            existingFornecedor.CNPJ = fornecedorDto.CNPJ;
            existingFornecedor.Telefone = fornecedorDto.Telefone;
            existingFornecedor.Endereco = fornecedorDto.Endereco;
            existingFornecedor.Email = fornecedorDto.Email;

            await _fornecedorService.UpdateFornecedorAsync(existingFornecedor);

            return NoContent();
        }

        /// <summary>
        /// Remove um fornecedor existente pelo ID.
        /// </summary>
        /// <param name="id">O ID do fornecedor a ser removido.</param>
        /// <returns>Retorna um status de "NoContent" se a remoção for bem-sucedida.</returns>
        [Authorize(Policy = "ManageFornecedores")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }

            var existingFornecedor = await _fornecedorService.GetByIdAsync(id);
            if (existingFornecedor == null)
            {
                return NotFound("Fornecedor não encontrado");
            }

            // Verificar se o fornecedor pertence ao estabelecimento do usuário
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(existingFornecedor.EstabelecimentoId);
            if (estabelecimento == null || estabelecimento.UsuarioId != userId)
            {
                return Forbid("Você não tem permissão para remover este fornecedor.");
            }

            await _fornecedorService.DeleteFornecedorAsync(id);

            return NoContent();
        }
    }
}