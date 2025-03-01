using Microsoft.AspNetCore.Mvc;
using Web.Application.Interfaces;
using Web.Domain.DTOs;

namespace Web.API.Controllers
{
    /// <summary>
    /// Controller para gerenciar os valores de opções de produtos
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ValorOpcaoProdutoController : ControllerBase
    {
        private readonly IValorOpcaoProdutoService _valorOpcaoProdutoService;
        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="valorOpcaoProdutoService"></param>
        public ValorOpcaoProdutoController(IValorOpcaoProdutoService valorOpcaoProdutoService)
        {
            _valorOpcaoProdutoService = valorOpcaoProdutoService;
        }
        /// <summary>
        /// Retorna todos os valores de uma opção de produto.
        /// </summary>
        /// <param name="estabelecimentoId"></param>
        /// <param name="produtoId"></param>
        /// <param name="opcaoId"></param>
        /// <returns></returns>
        [HttpGet("{estabelecimentoId:int}/{produtoId:int}/{opcaoId:int}/valores")]
        public async Task<IActionResult> GetAllByOpcao(int estabelecimentoId, int produtoId, int opcaoId)
        {
            var valores = await _valorOpcaoProdutoService.GetAllByOpcaoAsync(estabelecimentoId, produtoId, opcaoId);
            return Ok(valores);
        }
        /// <summary>
        /// Retorna um valor de uma opção de produto.
        /// </summary>
        /// <param name="estabelecimentoId"></param>
        /// <param name="produtoId"></param>
        /// <param name="opcaoId"></param>
        /// <param name="valorId"></param>
        /// <returns></returns>
        [HttpGet("{estabelecimentoId:int}/{produtoId:int}/{opcaoId:int}/valores/{valorId:int}")]
        public async Task<IActionResult> GetById(int estabelecimentoId, int produtoId, int opcaoId, int valorId)
        {
            var valor = await _valorOpcaoProdutoService.GetValorByIdAsync(estabelecimentoId, produtoId, opcaoId, valorId);
            return Ok(valor);
        }
        /// <summary>
        ///     Adiciona um novo valor a uma opção de produto.
        /// </summary>
        /// <param name="estabelecimentoId"></param>
        /// <param name="produtoId"></param>
        /// <param name="opcaoId"></param>
        /// <param name="valorDto"></param>
        /// <returns></returns>
        [HttpPost("{estabelecimentoId:int}/{produtoId:int}/{opcaoId:int}/valores")]
        public async Task<IActionResult> Create(int estabelecimentoId, int produtoId, int opcaoId, [FromBody] ValorOpcaoProdutoDto valorDto)
        {
            await _valorOpcaoProdutoService.AddValorAsync(estabelecimentoId, produtoId, opcaoId, valorDto);
            return Ok("Valor adicionado com sucesso");
        }
        /// <summary>
        /// Atualiza um valor de uma opção de produto.
        /// </summary>
        /// <param name="estabelecimentoId"></param>
        /// <param name="produtoId"></param>
        /// <param name="opcaoId"></param>
        /// <param name="valorId"></param>
        /// <param name="valorDto"></param>
        /// <returns></returns>
        [HttpPut("{estabelecimentoId:int}/{produtoId:int}/{opcaoId:int}/valores/{valorId:int}")]
        public async Task<IActionResult> Update(int estabelecimentoId, int produtoId, int opcaoId, int valorId, [FromBody] ValorOpcaoProdutoDto valorDto)
        {
            valorDto.Id = valorId;
            valorDto.OpcaoProdutoId = opcaoId;
            await _valorOpcaoProdutoService.UpdateValorAsync(estabelecimentoId, produtoId, opcaoId, valorDto);
            return Ok("Valor atualizado com sucesso");
        }
        /// <summary>
        /// Remove um valor de uma opção de produto.
        /// </summary>
        /// <param name="estabelecimentoId"></param>
        /// <param name="produtoId"></param>
        /// <param name="opcaoId"></param>
        /// <param name="valorId"></param>
        /// <returns></returns>
        [HttpDelete("{estabelecimentoId:int}/{produtoId:int}/{opcaoId:int}/valores/{valorId:int}")]
        public async Task<IActionResult> Delete(int estabelecimentoId, int produtoId, int opcaoId, int valorId)
        {
            await _valorOpcaoProdutoService.RemoveValorAsync(estabelecimentoId, produtoId, opcaoId, valorId);
            return Ok("Valor removido com sucesso");
        }
    }
}