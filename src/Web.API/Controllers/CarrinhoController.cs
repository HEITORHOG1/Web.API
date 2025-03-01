using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.Application.Interfaces;
using Web.Domain.DTOs;
using Web.Domain.Entities;
using Web.Domain.Enums;

namespace Web.API.Controllers
{
    /// <summary>
    /// Controller para gerenciar o carrinho de compras do usuário.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Cliente")]
    public class CarrinhoController : ControllerBase
    {
        private readonly ICarrinhoService _carrinhoService;
        private readonly IEstabelecimentoService _estabelecimentoService;
        private readonly IProdutoService _produtoService;

        /// <summary>
        /// Construtor padrão com injeção de dependência.
        /// </summary>
        /// <param name="carrinhoService"></param>
        /// <param name="estabelecimentoService"></param>
        /// <param name="produtoService"></param>
        public CarrinhoController(
            ICarrinhoService carrinhoService,
            IEstabelecimentoService estabelecimentoService,
            IProdutoService produtoService)
        {
            _carrinhoService = carrinhoService;
            _estabelecimentoService = estabelecimentoService;
            _produtoService = produtoService;
        }

        /// <summary>
        /// retorna o carrinho do usuário pelo id do usuário, se o usuário não estiver autenticado retorna uma mensagem de não autorizado
        /// pelo token de autenticação esta pegando o id do usuário e retornando os itens do carrinho ativos
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCarrinho()
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }

            var itens = await _carrinhoService.GetCarrinhoItensAsync(userId);
            var itensAtivos = itens.Where(i => i.Status == StatusCarrinhoItem.Ativo);

            return Ok(itensAtivos);
        }

        /// <summary>
        /// atualiza a quantidade de um item no carrinho, se o item não existir retorna uma mensagem de item não encontrado
        /// exemplo quando acrenceita um item no carrinho e depois quer aumentar a quantidade do item
        /// ou retirar um item do carrinho quando for retirar sempre -1 ou -2 e assim por diante
        /// </summary>
        /// <param name="itemDto"></param>
        /// <returns></returns>
        [HttpPut("atualizar-quantidade")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AtualizarQuantidade([FromBody] AtualizarQuantidadeDto itemDto)
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

            // Verificar se o item existe no carrinho
            var item = await _carrinhoService.GetItemAsync(userId, itemDto.ProdutoId, itemDto.EstabelecimentoId);
            if (item == null || item.Status != StatusCarrinhoItem.Ativo)
            {
                return NotFound("Item não encontrado ou já foi removido.");
            }

            // Calcular a nova quantidade
            var novaQuantidade = item.Quantidade + itemDto.Quantidade;

            // Verificar se a nova quantidade é válida
            if (novaQuantidade < 0)
            {
                return BadRequest("A quantidade não pode ser menor que zero.");
            }

            // Atualizar a quantidade
            item.Quantidade = novaQuantidade;

            // Se a quantidade for zero, remover o item do carrinho
            if (item.Quantidade == 0)
            {
                item.Status = StatusCarrinhoItem.Removido;
            }

            await _carrinhoService.AtualizarItemAsync(item);

            return Ok("Quantidade atualizada com sucesso.");
        }

        /// <summary>
        /// limpa o carrinho do usuário, se o usuário não estiver autenticado retorna uma mensagem de não autorizado
        /// e uma remoção logicamente dos itens do carrinho
        /// </summary>
        /// <returns></returns>
        [HttpDelete("limpar-carrinho")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LimparCarrinho()
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }

            var itens = await _carrinhoService.GetCarrinhoItensAsync(userId);
            foreach (var item in itens.Where(i => i.Status == StatusCarrinhoItem.Ativo))
            {
                item.Status = StatusCarrinhoItem.Removido;
                await _carrinhoService.AtualizarItemAsync(item);
            }

            return Ok("Carrinho limpo com sucesso.");
        }

        /// <summary>
        /// adiciona um item ou mais itens ao carrinho, se o usuário não estiver autenticado retorna uma mensagem de não autorizado
        /// </summary>
        /// <param name="itensDto"></param>
        /// <returns></returns>
        [HttpPost("adicionar-multiplos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AdicionarMultiplosItensAoCarrinho([FromBody] List<CarrinhoItemDto> itensDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado.");
            }

            try
            {
                // Verificar se o estabelecimento existe
                var estabelecimentoId = itensDto.First().EstabelecimentoId; // Assume que todos os itens são do mesmo estabelecimento
                var estabelecimento = await _estabelecimentoService.GetByIdAsync(estabelecimentoId);
                if (estabelecimento == null)
                {
                    return NotFound("Estabelecimento não encontrado.");
                }

                // Verificar se o estabelecimento está aberto
                var horarioAtual = DateTime.UtcNow.TimeOfDay.Add(TimeSpan.FromHours(-3)); // Ajuste para o fuso horário local
                if (horarioAtual < TimeSpan.Zero)
                {
                    horarioAtual = horarioAtual.Add(TimeSpan.FromHours(24));
                }

                if (!await _estabelecimentoService.EstaAbertoAsync(estabelecimentoId, DateTime.UtcNow.DayOfWeek, horarioAtual))
                {
                    return BadRequest("O estabelecimento está fechado no momento.");
                }

                // Verificar se todos os produtos existem e estão disponíveis
                foreach (var itemDto in itensDto)
                {
                    var produto = await _produtoService.GetProdutoByIdAsync(itemDto.EstabelecimentoId, itemDto.ProdutoId);
                    if (produto == null || !produto.Disponivel)
                    {
                        return BadRequest($"Produto {itemDto.ProdutoId} indisponível.");
                    }

                    if (produto.QuantidadeEmEstoque < itemDto.Quantidade)
                    {
                        return BadRequest($"Estoque insuficiente para o produto {itemDto.ProdutoId}.");
                    }
                }

                // Processar cada item
                foreach (var itemDto in itensDto)
                {
                    // Verificar se o item já está no carrinho
                    var itemExistente = await _carrinhoService.GetItemAsync(userId, itemDto.ProdutoId, itemDto.EstabelecimentoId);
                    if (itemExistente != null)
                    {
                        // Incrementar a quantidade
                        itemExistente.Quantidade += itemDto.Quantidade;
                        itemExistente.Status = StatusCarrinhoItem.Ativo;
                        await _carrinhoService.AtualizarItemAsync(itemExistente);
                    }
                    else
                    {
                        // Adicionar novo item ao carrinho
                        var novoItem = new CarrinhoItem
                        {
                            UsuarioId = userId,
                            EstabelecimentoId = itemDto.EstabelecimentoId,
                            ProdutoId = itemDto.ProdutoId,
                            Quantidade = itemDto.Quantidade,
                            DataAdicionado = DateTime.UtcNow,
                            Status = StatusCarrinhoItem.Ativo
                        };
                        await _carrinhoService.AddItemAoCarrinhoAsync(userId, novoItem.EstabelecimentoId, novoItem.ProdutoId, novoItem.Quantidade);
                    }
                }

                return Ok(new { Message = "Itens adicionados ao carrinho com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = ex.Message });
            }
        }

        /// <summary>
        /// atualiza a quantidade de um item ou mais itens no carrinho, se o usuário não estiver autenticado retorna uma mensagem de não autorizado
        /// </summary>
        /// <param name="itensDto"></param>
        /// <returns></returns>
        [HttpPut("atualizar-multiplos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AtualizarMultiplosItensNoCarrinho([FromBody] List<CarrinhoItemDto> itensDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado.");
            }

            try
            {
                // Verificar se o estabelecimento existe
                var estabelecimentoId = itensDto.First().EstabelecimentoId; // Assume que todos os itens são do mesmo estabelecimento
                var estabelecimento = await _estabelecimentoService.GetByIdAsync(estabelecimentoId);
                if (estabelecimento == null)
                {
                    return NotFound("Estabelecimento não encontrado.");
                }

                // Verificar se o estabelecimento está aberto
                var horarioAtual = DateTime.UtcNow.TimeOfDay.Add(TimeSpan.FromHours(-3)); // Ajuste para o fuso horário local
                if (horarioAtual < TimeSpan.Zero)
                {
                    horarioAtual = horarioAtual.Add(TimeSpan.FromHours(24));
                }

                if (!await _estabelecimentoService.EstaAbertoAsync(estabelecimentoId, DateTime.UtcNow.DayOfWeek, horarioAtual))
                {
                    return BadRequest("O estabelecimento está fechado no momento.");
                }

                // Verificar se todos os produtos existem e estão disponíveis
                foreach (var itemDto in itensDto)
                {
                    var produto = await _produtoService.GetProdutoByIdAsync(itemDto.EstabelecimentoId, itemDto.ProdutoId);
                    if (produto == null || !produto.Disponivel)
                    {
                        return BadRequest($"Produto {itemDto.ProdutoId} indisponível.");
                    }

                    if (produto.QuantidadeEmEstoque < itemDto.Quantidade)
                    {
                        return BadRequest($"Estoque insuficiente para o produto {itemDto.ProdutoId}.");
                    }
                }

                // Processar cada item
                foreach (var itemDto in itensDto)
                {
                    // Verificar se o item existe no carrinho
                    var itemExistente = await _carrinhoService.GetItemAsync(userId, itemDto.ProdutoId, itemDto.EstabelecimentoId);
                    if (itemExistente == null)
                    {
                        return NotFound($"Item {itemDto.ProdutoId} não encontrado no carrinho.");
                    }

                    // Atualizar a quantidade
                    itemExistente.Quantidade += itemDto.Quantidade;
                    await _carrinhoService.AtualizarItemAsync(itemExistente);
                }

                return Ok(new { Message = "Itens atualizados no carrinho com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = ex.Message });
            }
        }
    }
}