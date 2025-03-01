using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.Application.Interfaces;
using Web.Domain.DTOs.NotaFiscal;
using Web.Domain.Entities;

namespace Web.API.Controllers
{
    /// <summary>
    /// Controller para gerenciar notas fiscais.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class NotasFiscaisController : ControllerBase
    {
        private readonly INotaFiscalService _notaFiscalService;
        private readonly IProdutoService _produtoService;
        private readonly IEstabelecimentoService _estabelecimentoService;
        private readonly IFornecedorService _fornecedorService;
        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="notaFiscalService"></param>
        /// <param name="produtoService"></param>
        /// <param name="estabelecimentoService"></param>
        /// <param name="fornecedorService"></param>
        public NotasFiscaisController(
            INotaFiscalService notaFiscalService,
            IProdutoService produtoService,
            IEstabelecimentoService estabelecimentoService,
            IFornecedorService fornecedorService)
        {
            _notaFiscalService = notaFiscalService;
            _produtoService = produtoService;
            _estabelecimentoService = estabelecimentoService;
            _fornecedorService = fornecedorService;
        }

        /// <summary>
        /// Registra uma nova nota fiscal associada a um estabelecimento.
        /// </summary>
        /// <param name="estabelecimentoId">ID do estabelecimento ao qual a nota fiscal será associada.</param>
        /// <param name="notaFiscalDto">Os dados da nota fiscal a ser criada.</param>
        /// <returns>Retorna a nota fiscal criada com um status de criação (201).</returns>
        [Authorize(Roles = "Proprietario,Gerente")]
        [HttpPost("estabelecimentos/{estabelecimentoId:int}/notas-fiscais")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegistrarNotaFiscal(int estabelecimentoId, [FromBody] NotaFiscalDto notaFiscalDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Retorna as mensagens de validação automaticamente
            }

            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }

            // Verificar se o usuário tem permissão para registrar notas fiscais neste estabelecimento
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(estabelecimentoId);
            if (estabelecimento == null || estabelecimento.UsuarioId != userId)
            {
                return Forbid("Você não tem permissão para registrar notas fiscais neste estabelecimento.");
            }

            // Verificar se o fornecedor existe e pertence ao estabelecimento
            var fornecedor = await _fornecedorService.GetByIdAsync(notaFiscalDto.FornecedorId);
            if (fornecedor == null || fornecedor.EstabelecimentoId != estabelecimentoId)
            {
                return NotFound("Fornecedor não encontrado ou não pertence a este estabelecimento.");
            }

            // Converter o DTO para a entidade NotaFiscal
            var notaFiscal = new NotaFiscal
            {
                EstabelecimentoId = estabelecimentoId,
                IdUsuario = userId,
                FornecedorId = notaFiscalDto.FornecedorId,
                Numero = notaFiscalDto.Numero,
                DataEmissao = notaFiscalDto.DataEmissao,
                ValorTotal = notaFiscalDto.Produtos.Sum(p => p.Quantidade * p.PrecoUnitario),
                Produtos = notaFiscalDto.Produtos.Select(p => new NotaFiscalProduto
                {
                    ProdutoId = p.ProdutoId,
                    Quantidade = p.Quantidade,
                    PrecoUnitario = p.PrecoUnitario
                }).ToList()
            };

            await _notaFiscalService.AddNotaFiscalAsync(notaFiscal);

            return CreatedAtAction(nameof(GetNotaFiscalById), new { id = notaFiscal.Id }, notaFiscal);
        }

        /// <summary>
        /// Obtém os detalhes de uma nota fiscal específica pelo ID.
        /// </summary>
        /// <param name="id">O ID da nota fiscal a ser buscada.</param>
        /// <returns>Retorna os detalhes da nota fiscal encontrada ou uma mensagem de erro se não for encontrada.</returns>
        [Authorize(Roles = "Proprietario,Gerente")]
        [HttpGet("notas-fiscais/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetNotaFiscalById(int id)
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }
            var notaFiscal = await _notaFiscalService.GetByIdAsync(id);
            if (notaFiscal == null)
            {
                return NotFound("Nota fiscal não encontrada");
            }

            return Ok(notaFiscal);
        }

        /// <summary>
        /// Atualiza uma nota fiscal existente.
        /// </summary>
        /// <param name="id">O ID da nota fiscal a ser atualizada.</param>
        /// <param name="notaFiscalDto">Os novos dados da nota fiscal.</param>
        /// <returns>Retorna um status de "NoContent" se a atualização for bem-sucedida.</returns>
        [Authorize(Roles = "Proprietario,Gerente")]
        [HttpPut("notas-fiscais/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateNotaFiscal(int id, [FromBody] NotaFiscalDto notaFiscalDto, int estabelecimentoId)
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

            var existingNotaFiscal = await _notaFiscalService.GetByIdAsync(id);
            if (existingNotaFiscal == null)
            {
                return NotFound("Nota fiscal não encontrada");
            }

            // Verificar se o usuário tem permissão para atualizar esta nota fiscal
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(existingNotaFiscal.EstabelecimentoId);
            if (estabelecimento == null || estabelecimento.UsuarioId != userId)
            {
                return Forbid("Você não tem permissão para atualizar esta nota fiscal.");
            }

            // Verificar se o fornecedor existe e pertence ao estabelecimento
            var fornecedor = await _fornecedorService.GetByIdAsync(notaFiscalDto.FornecedorId);
            if (fornecedor == null || fornecedor.EstabelecimentoId != estabelecimentoId)
            {
                return NotFound("Fornecedor não encontrado ou não pertence a este estabelecimento.");
            }

            // Atualizar os dados da nota fiscal existente com os dados do DTO
            existingNotaFiscal.Numero = notaFiscalDto.Numero;
            existingNotaFiscal.DataEmissao = notaFiscalDto.DataEmissao;
            existingNotaFiscal.FornecedorId = notaFiscalDto.FornecedorId;
            existingNotaFiscal.Produtos.Clear();
            foreach (var item in notaFiscalDto.Produtos)
            {
                var notaFiscalProduto = new NotaFiscalProduto
                {
                    ProdutoId = item.ProdutoId,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = item.PrecoUnitario
                };
                existingNotaFiscal.Produtos.Add(notaFiscalProduto);
            }

            await _notaFiscalService.UpdateNotaFiscalAsync(existingNotaFiscal);

            return NoContent();
        }

        /// <summary>
        /// Remove uma nota fiscal existente pelo ID.
        /// </summary>
        /// <param name="id">O ID da nota fiscal a ser removida.</param>
        /// <returns>Retorna um status de "NoContent" se a remoção for bem-sucedida.</returns>
        [Authorize(Roles = "Proprietario,Gerente")]
        [HttpDelete("notas-fiscais/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteNotaFiscal(int id)
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }

            var existingNotaFiscal = await _notaFiscalService.GetByIdAsync(id);
            if (existingNotaFiscal == null)
            {
                return NotFound("Nota fiscal não encontrada");
            }

            await _notaFiscalService.DeleteNotaFiscalAsync(id);

            return NoContent();
        }

        /// <summary>
        /// Obtém todas as notas fiscais de um estabelecimento específico.
        /// </summary>
        /// <param name="estabelecimentoId">ID do estabelecimento.</param>
        /// <returns>Lista de notas fiscais do estabelecimento.</returns>
        [Authorize(Roles = "Proprietario,Gerente")]
        [HttpGet("estabelecimentos/{estabelecimentoId:int}/notas-fiscais")]
        [ProducesResponseType(typeof(IEnumerable<NotaFiscal>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetNotasFiscaisByEstabelecimento(int estabelecimentoId)
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }

            // Verificar se o usuário tem permissão para visualizar notas fiscais neste estabelecimento
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(estabelecimentoId);
            if (estabelecimento == null || estabelecimento.UsuarioId != userId)
            {
                return Forbid("Você não tem permissão para visualizar notas fiscais neste estabelecimento.");
            }

            var notasFiscais = await _notaFiscalService.GetAllByEstabelecimentoIdAsync(estabelecimentoId);
            return Ok(notasFiscais);
        }
    }
}