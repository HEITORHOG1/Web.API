using Microsoft.AspNetCore.Mvc;
using Web.Application.Interfaces;
using Web.Domain.DTOs;

namespace Web.API.Controllers
{
    /// <summary>
    /// Controller responsável por gerenciar as opções de produtos
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OpcaoProdutoController : ControllerBase
    {
        private readonly IOpcaoProdutoService _opcaoProdutoService;
        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="opcaoProdutoService"></param>
        public OpcaoProdutoController(IOpcaoProdutoService opcaoProdutoService)
        {
            _opcaoProdutoService = opcaoProdutoService;
        }
        /// <summary>
        /// Retorna todas as opções de um produto.
        /// </summary>
        /// <param name="estabelecimentoId"></param>
        /// <param name="produtoId"></param>
        /// <returns></returns>
        [HttpGet("{estabelecimentoId:int}/{produtoId:int}")]
        public async Task<IActionResult> GetAll(int estabelecimentoId, int produtoId)
        {
            var opcoes = await _opcaoProdutoService.GetAllByProdutoIdAsync(estabelecimentoId, produtoId);
            return Ok(opcoes);
        }
        /// <summary>
        ///     Retorna uma opção de um produto.
        /// </summary>
        /// <param name="estabelecimentoId"></param>
        /// <param name="produtoId"></param>
        /// <param name="opcaoId"></param>
        /// <returns></returns>
        [HttpGet("{estabelecimentoId:int}/{produtoId:int}/{opcaoId:int}")]
        public async Task<IActionResult> GetById(int estabelecimentoId, int produtoId, int opcaoId)
        {
            var opcao = await _opcaoProdutoService.GetOpcaoByIdAsync(estabelecimentoId, produtoId, opcaoId);
            return Ok(opcao);
        }
        /// <summary>
        /// Adiciona uma nova opção a um produto.
        /// </summary>
        /// <param name="estabelecimentoId"></param>
        /// <param name="produtoId"></param>
        /// <param name="opcaoDto"></param>
        /// <returns></returns>
        [HttpPost("{estabelecimentoId:int}/{produtoId:int}")]
        public async Task<IActionResult> Create(int estabelecimentoId, int produtoId, [FromBody] OpcaoProdutoDto opcaoDto)
        {
            await _opcaoProdutoService.AddOpcaoAsync(estabelecimentoId, produtoId, opcaoDto);
            return Ok("Opção adicionada com sucesso");
        }
        /// <summary>
        ///     Atualiza uma opção de um produto.
        /// </summary>
        /// <param name="estabelecimentoId"></param>
        /// <param name="produtoId"></param>
        /// <param name="opcaoId"></param>
        /// <param name="opcaoDto"></param>
        /// <returns></returns>
        [HttpPut("{estabelecimentoId:int}/{produtoId:int}/{opcaoId:int}")]
        public async Task<IActionResult> Update(int estabelecimentoId, int produtoId, int opcaoId, [FromBody] OpcaoProdutoDto opcaoDto)
        {
            opcaoDto.Id = opcaoId;
            opcaoDto.ProdutoId = produtoId;
            await _opcaoProdutoService.UpdateOpcaoAsync(estabelecimentoId, produtoId, opcaoDto);
            return Ok("Opção atualizada com sucesso");
        }
        /// <summary>
        ///    Remove uma opção de um produto.
        /// </summary>
        /// <param name="estabelecimentoId"></param>
        /// <param name="produtoId"></param>
        /// <param name="opcaoId"></param>
        /// <returns></returns>
        [HttpDelete("{estabelecimentoId:int}/{produtoId:int}/{opcaoId:int}")]
        public async Task<IActionResult> Delete(int estabelecimentoId, int produtoId, int opcaoId)
        {
            await _opcaoProdutoService.RemoveOpcaoAsync(estabelecimentoId, produtoId, opcaoId);
            return Ok("Opção removida com sucesso");
        }

        /// <summary>
        /// Replica as opções de um produto para todos os produtos de uma categoria.
        /// </summary>
        /// <param name="categoriaId"></param>
        /// <param name="produtoOrigemId"></param>
        /// <returns></returns>
        [HttpPost("replicar/{categoriaId:int}/{produtoOrigemId:int}")]
        public async Task<IActionResult> ReplicarOpcoes(int categoriaId, int produtoOrigemId)
        {
            try
            {
                await _opcaoProdutoService.ReplicarOpcoesParaProdutosDaCategoriaAsync(categoriaId, produtoOrigemId);
                return Ok("Opções replicadas com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao replicar opções: {ex.Message}");
            }
        }
    }
}