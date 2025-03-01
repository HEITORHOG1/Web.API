using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.Application.Interfaces;
using Web.Domain.DTOs;
using Web.Domain.Entities;

namespace Web.API.Controllers
{
    /// <summary>
    /// Controller para gerenciar movimentações de estoque.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MovimentacaoEstoqueController : ControllerBase
    {
        private readonly IMovimentacaoEstoqueService _movimentacaoEstoqueService;
        private readonly IProdutoService _produtoService;
        private readonly IUsuarioEstabelecimentoService _usuarioEstabelecimentoService;
        private readonly IEstabelecimentoService _estabelecimentoService;
        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="movimentacaoEstoqueService"></param>
        /// <param name="produtoService"></param>
        /// <param name="usuarioEstabelecimentoService"></param>
        /// <param name="estabelecimentoService"></param>
        public MovimentacaoEstoqueController(
            IMovimentacaoEstoqueService movimentacaoEstoqueService,
            IProdutoService produtoService,
            IUsuarioEstabelecimentoService usuarioEstabelecimentoService,
            IEstabelecimentoService estabelecimentoService)
        {
            _movimentacaoEstoqueService = movimentacaoEstoqueService;
            _produtoService = produtoService;
            _usuarioEstabelecimentoService = usuarioEstabelecimentoService;
            _estabelecimentoService = estabelecimentoService;
        }

        /// <summary>
        /// Registra uma nova movimentação de estoque para um produto em um estabelecimento específico.
        /// </summary>
        /// <param name="estabelecimentoId">ID do estabelecimento.</param>
        /// <param name="movimentacaoDto">Dados da movimentação de estoque.</param>
        /// <returns>Movimentação de estoque criada.</returns>
        [Authorize(Roles = "Proprietario,Gerente")]
        [HttpPost("estabelecimentos/{estabelecimentoId:int}/movimentacoes")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegistrarMovimentacao(int estabelecimentoId, [FromBody] MovimentacaoEstoqueDto movimentacaoDto)
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

            // Verificar se o usuário tem permissão para este estabelecimento
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(estabelecimentoId);
            if (estabelecimento == null || estabelecimento.UsuarioId != userId)
            {
                return Forbid("Você não tem permissão para registrar movimentações neste estabelecimento.");
            }

            // Verificar se o produto pertence ao estabelecimento
            var produto = await _produtoService.GetProdutoByIdAsync(estabelecimentoId, movimentacaoDto.ProdutoId);
            if (produto == null)
            {
                return NotFound("Produto não encontrado no estabelecimento especificado.");
            }

            if (movimentacaoDto.Quantidade <= 0)
            {
                return BadRequest("A quantidade deve ser maior que zero.");
            }

            try
            {
                MovimentacaoEstoque movimentacao = new MovimentacaoEstoque
                {
                    ProdutoId = movimentacaoDto.ProdutoId,
                    EstabelecimentoId = estabelecimentoId,
                    Quantidade = movimentacaoDto.Quantidade,
                    Tipo = movimentacaoDto.Tipo,
                    Observacao = movimentacaoDto.Observacao,
                    DataMovimentacao = DateTime.UtcNow
                };

                // Registrar a movimentação
                await _movimentacaoEstoqueService.RegistrarMovimentacaoAsync(movimentacao);

                // Obter a movimentação recém-criada para retornar ao cliente
                var movimentacaoCriada = await _movimentacaoEstoqueService.GetByIdAsync(movimentacao.Id);

                return CreatedAtAction(nameof(GetById), new { id = movimentacaoCriada.Id }, movimentacaoCriada);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obtém uma movimentação de estoque específica pelo ID.
        /// </summary>
        /// <param name="id">ID da movimentação de estoque.</param>
        /// <returns>Movimentação de estoque encontrada.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var movimentacao = await _movimentacaoEstoqueService.GetMovimentacaoByIdAsync(id);
            if (movimentacao == null)
            {
                return NotFound("Movimentação não encontrada");
            }

            return Ok(movimentacao);
        }
    }
}