using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.Application.Interfaces;
using Web.Domain.DTOs;
using Web.Domain.Entities;

namespace Web.API.Controllers
{
    /// <summary>
    /// Controller para gerenciar entregadores.
    /// </summary>
    [Route("api/entregadores")]
    [ApiController]
    public class EntregadorController : ControllerBase
    {
        private readonly IEntregadorService _entregadorService;
        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="entregadorService"></param>
        public EntregadorController(IEntregadorService entregadorService)
        {
            _entregadorService = entregadorService;
        }

        /// <summary>
        /// Cria um novo entregador para o estabelecimento.
        /// </summary>
        /// <param name="estabelecimentoId">ID do estabelecimento.</param>
        /// <param name="entregadorDto">Dados do entregador a ser criado.</param>
        /// <returns>Entregador criado.</returns>
        [Authorize(Roles = "Proprietario,Administrador")]
        [Authorize(Policy = "GerenciarEstabelecimento")]
        [HttpPost("{estabelecimentoId:int}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CriarEntregador(int estabelecimentoId, [FromBody] EntregadorDto entregadorDto)
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

            var entregador = new Entregador
            {
                Nome = entregadorDto.Nome,
                Telefone = entregadorDto.Telefone,
                Veiculo = entregadorDto.Veiculo,
                PlacaVeiculo = entregadorDto.PlacaVeiculo,
                Documento = entregadorDto.Documento,
                EstabelecimentoId = estabelecimentoId
            };

            await _entregadorService.AddAsync(entregador);
            return CreatedAtAction(nameof(GetEntregadorPorId), new { estabelecimentoId, entregadorId = entregador.Id }, entregador);
        }

        /// <summary>
        /// Obtém um entregador específico de um estabelecimento.
        /// </summary>
        /// <param name="estabelecimentoId">ID do estabelecimento.</param>
        /// <param name="entregadorId">ID do entregador.</param>
        /// <returns>O entregador encontrado ou uma mensagem de erro se não for encontrado.</returns>
        [Authorize(Roles = "Proprietario,Administrador")]
        [Authorize(Policy = "GerenciarEstabelecimento")]
        [HttpGet("{estabelecimentoId:int}/{entregadorId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEntregadorPorId(int estabelecimentoId, int entregadorId)
        {
            var entregador = await _entregadorService.GetByIdAsync(entregadorId);
            if (entregador == null || entregador.EstabelecimentoId != estabelecimentoId)
            {
                return NotFound("Entregador não encontrado ou não pertence a este estabelecimento");
            }

            return Ok(entregador);
        }

        /// <summary>
        /// Obtém todos os entregadores de um estabelecimento.
        /// </summary>
        /// <param name="estabelecimentoId">ID do estabelecimento.</param>
        /// <returns>Lista de entregadores.</returns>
        [Authorize(Roles = "Proprietario,Administrador")]
        [Authorize(Policy = "GerenciarEstabelecimento")]
        [HttpGet("{estabelecimentoId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEntregadores(int estabelecimentoId)
        {
            var entregadores = await _entregadorService.GetEntregadoresByEstabelecimentoIdAsync(estabelecimentoId);
            return Ok(entregadores);
        }
    }
}