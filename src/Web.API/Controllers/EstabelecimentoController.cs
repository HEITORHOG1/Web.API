using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.Application;
using Web.Application.Interfaces;
using Web.Domain.DTOs;
using Web.Domain.DTOs.Produtos;
using Web.Domain.Entities;
using Web.Domain.Enums;
using Web.Domain.Paginacao;

namespace Web.API.Controllers
{
    /// <summary>
    /// Controller para gerenciar estabelecimentos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EstabelecimentoController : ControllerBase
    {
        private readonly IEstabelecimentoService _estabelecimentoService;
        private readonly IPedidoService _pedidoService;
        private readonly IMovimentacaoEstoqueService _movimentacaoEstoqueService;
        private readonly IProdutoService _produtoService;
        private readonly IEntregadorService _entregadorService; // Adicionado
        private readonly IEntregaService _entregaService; // Adicionado
        private readonly IHorarioFuncionamentoService _horarioFuncionamentoService; // Adicionado
        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="estabelecimentoService"></param>
        /// <param name="pedidoService"></param>
        /// <param name="produtoService"></param>
        /// <param name="movimentacaoEstoqueService"></param>
        /// <param name="entregadorService"></param>
        /// <param name="entregaService"></param>
        /// <param name="horarioFuncionamentoService"></param>
        public EstabelecimentoController(
            IEstabelecimentoService estabelecimentoService,
            IPedidoService pedidoService,
            IProdutoService produtoService,
            IMovimentacaoEstoqueService movimentacaoEstoqueService,
            IEntregadorService entregadorService, // Adicionado
            IEntregaService entregaService,
            IHorarioFuncionamentoService horarioFuncionamentoService
        )
        {
            _estabelecimentoService = estabelecimentoService;
            _pedidoService = pedidoService;
            _produtoService = produtoService;
            _movimentacaoEstoqueService = movimentacaoEstoqueService;
            _entregadorService = entregadorService; // Adicionado
            _entregaService = entregaService; // Adicionado
            _horarioFuncionamentoService = horarioFuncionamentoService; // Adicionado
        }

        /// <summary>
        /// Obtém todos os estabelecimentos de um proprietário específico.
        /// </summary>
        /// <param name="proprietarioId">O ID do proprietário.</param>
        /// <returns>Uma lista de estabelecimentos do proprietário.</returns>
        [Authorize(Roles = "Proprietario,Administrador")]
        [HttpGet("proprietario/{proprietarioId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetEstabelecimentosByProprietarioId(string proprietarioId)
        {
            var estabelecimentos = await _estabelecimentoService.GetAllByProprietarioIdAsync(proprietarioId);
            return Ok(estabelecimentos);
        }

        /// <summary>
        /// Obtém todos os usuários de um estabelecimento específico.
        /// </summary>
        /// <param name="estabelecimentoId">O ID do estabelecimento.</param>
        /// <returns>Uma lista de usuários do estabelecimento.</returns>
        [Authorize(Roles = "Proprietario,Administrador")]
        [HttpGet("{estabelecimentoId:int}/usuarios")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsuariosByEstabelecimentoId(int estabelecimentoId)
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }

            // Verificar se o usuário é proprietário do estabelecimento
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(estabelecimentoId);
            if (estabelecimento == null)
            {
                return NotFound("Estabelecimento não encontrado");
            }

            if (estabelecimento.UsuarioId != userId)
            {
                return Forbid("Usuário não autorizado a acessar este estabelecimento");
            }

            var usuarios = await _estabelecimentoService.GetAllUsersByEstabelecimentoIdAsync(estabelecimentoId);
            return Ok(usuarios);
        }

        /// <summary>
        /// Obtém um estabelecimento específico pelo ID.
        /// </summary>
        /// <param name="id">O ID do estabelecimento a ser buscado.</param>
        /// <returns>O estabelecimento encontrado ou uma mensagem de erro se não for encontrado.</returns>
        [Authorize(Policy = "GerenciarEstabelecimento")]
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(id);
            if (estabelecimento == null)
            {
                return NotFound("Estabelecimento não encontrado");
            }
            return Ok(estabelecimento);
        }

        /// <summary>
        /// Cria um novo estabelecimento associado ao usuário autenticado.
        /// </summary>
        /// <param name="estabelecimentoDto">Os dados do estabelecimento a ser criado.</param>
        /// <returns>O estabelecimento recém-criado com um status de criação (201).</returns>
        [Authorize(Roles = "Proprietario,Administrador")]
        [Authorize(Policy = "GerenciarEstabelecimento")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromForm] CreateEstabelecimentoDto estabelecimentoDto)
        {
            try
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

                var estabelecimentoCriado = await _estabelecimentoService.AddWithUserAsync(estabelecimentoDto, userId);

                var response = new
                {
                    estabelecimentoCriado.Id,
                    estabelecimentoCriado.NomeFantasia,
                    Message = "Estabelecimento criado com sucesso"
                };

                return CreatedAtAction(nameof(GetById), new { id = estabelecimentoCriado.Id }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno ao criar estabelecimento: " + ex.Message);
            }
        }

        /// <summary>
        /// Atualiza um estabelecimento existente associado ao usuário autenticado.
        /// </summary>
        /// <param name="id">O ID do estabelecimento a ser atualizado.</param>
        /// <param name="estabelecimento">Os dados atualizados do estabelecimento.</param>
        /// <returns>Status de "NoContent" se a atualização for bem-sucedida.</returns>
        [Authorize(Roles = "Proprietario,Administrador")]
        [Authorize(Policy = "GerenciarEstabelecimento")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateEstabelecimento estabelecimento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingEstabelecimento = await _estabelecimentoService.GetByIdAsync(id);
            if (existingEstabelecimento == null)
            {
                return NotFound("Estabelecimento não encontrado");
            }

            var userId = User.FindFirstValue("UserId");
            if (existingEstabelecimento.UsuarioId != userId)
            {
                return Forbid("Usuário não autorizado a modificar este estabelecimento");
            }

            await _estabelecimentoService.UpdateAsync(estabelecimento);
            return NoContent();
        }

        /// <summary>
        /// Exclui um estabelecimento associado ao usuário autenticado.
        /// </summary>
        /// <param name="id">O ID do estabelecimento a ser excluído.</param>
        /// <returns>Status de "NoContent" se a exclusão for bem-sucedida.</returns>
        [Authorize(Roles = "Proprietario,Administrador")]
        [Authorize(Policy = "GerenciarEstabelecimento")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(id);
            if (estabelecimento == null)
            {
                return NotFound("Estabelecimento não encontrado");
            }

            var userId = User.FindFirstValue("UserId");
            if (estabelecimento.UsuarioId != userId)
            {
                return Forbid("Usuário não autorizado a deletar este estabelecimento");
            }

            await _estabelecimentoService.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Obtém todos os pedidos recebidos pelo estabelecimento autenticado.
        /// </summary>
        /// <param name="estabelecimentoId">ID do estabelecimento.</param>
        /// <returns>Uma lista de pedidos recebidos.</returns>
        [Authorize(Roles = "Proprietario,Administrador")]
        [Authorize(Policy = "GerenciarEstabelecimento")]
        [HttpGet("{estabelecimentoId:int}/pedidos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPedidosRecebidos(int estabelecimentoId)
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }

            // Verificar se o usuário é proprietário ou gerente do estabelecimento
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(estabelecimentoId);
            if (estabelecimento == null)
            {
                return NotFound("Estabelecimento não encontrado");
            }

            if (estabelecimento.UsuarioId != userId)
            {
                return Forbid("Usuário não autorizado a acessar este estabelecimento");
            }

            var pedidos = await _pedidoService.GetPedidosByEstabelecimentoIdAsync(estabelecimentoId);
            return Ok(pedidos);
        }

        /// <summary>
        /// Atualiza o status de um pedido recebido.
        /// </summary>
        /// <param name="estabelecimentoId">ID do estabelecimento.</param>
        /// <param name="pedidoId">ID do pedido.</param>
        /// <param name="novoStatus">Novo status do pedido.</param>
        /// <returns>Status de sucesso ou erro.</returns>
        [Authorize(Roles = "Proprietario,Administrador")]
        [Authorize(Policy = "GerenciarEstabelecimento")]
        [HttpPut("{estabelecimentoId:int}/pedidos/{pedidoId:int}/atualizar-status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AtualizarStatusPedido(int estabelecimentoId, int pedidoId, [FromBody] StatusPedido novoStatus)
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

            // Verificar se o usuário é proprietário ou gerente do estabelecimento
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(estabelecimentoId);
            if (estabelecimento == null)
            {
                return NotFound("Estabelecimento não encontrado");
            }

            if (estabelecimento.UsuarioId != userId)
            {
                return Forbid("Usuário não autorizado a acessar este estabelecimento");
            }

            // Verificar se o pedido pertence ao estabelecimento
            var pedido = await _pedidoService.GetPedidoByIdAsync(pedidoId);
            if (pedido == null || pedido.EstabelecimentoId != estabelecimentoId)
            {
                return NotFound("Pedido não encontrado ou não pertence a este estabelecimento");
            }

            try
            {
                await _pedidoService.AtualizarStatusPedidoAsync(pedidoId, novoStatus);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Cria um novo produto para o estabelecimento.
        /// </summary>
        /// <param name="estabelecimentoId">ID do estabelecimento.</param>
        /// <param name="produtoDto">Dados do produto a ser criado.</param>
        /// <returns>Produto criado.</returns>
        [Authorize(Roles = "Proprietario,Administrador")]
        [Authorize(Policy = "GerenciarEstabelecimento")]
        [HttpPost("{estabelecimentoId:int}/produtos")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CriarProduto(int estabelecimentoId, [FromBody] ProdutoDto produtoDto)
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

            // Verificar se o usuário é proprietário ou gerente do estabelecimento
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(estabelecimentoId);
            if (estabelecimento == null)
            {
                return NotFound("Estabelecimento não encontrado");
            }

            if (estabelecimento.UsuarioId != userId)
            {
                return Forbid("Usuário não autorizado a acessar este estabelecimento");
            }

            var produto = new Produto
            {
                Nome = produtoDto.Nome,
                Descricao = produtoDto.Descricao,
                Preco = produtoDto.Preco,
                QuantidadeEmEstoque = produtoDto.QuantidadeEmEstoque,
                CategoriaId = produtoDto.CategoriaId,
                EstabelecimentoId = estabelecimentoId
            };

            // Processar Opções
            if (produtoDto.Opcoes != null && produtoDto.Opcoes.Any())
            {
                produto.Opcoes = produtoDto.Opcoes.Select(opDto => new OpcaoProduto
                {
                    Nome = opDto.Nome,
                    Obrigatorio = opDto.Obrigatorio,
                    Valores = opDto.Valores.Select(vopDto => new ValorOpcaoProduto
                    {
                        Descricao = vopDto.Descricao,
                        PrecoAdicional = vopDto.PrecoAdicional
                    }).ToList()
                }).ToList();
            }

            // Processar Adicionais
            if (produtoDto.Adicionais != null && produtoDto.Adicionais.Any())
            {
                produto.Adicionais = produtoDto.Adicionais.Select(apDto => new AdicionalProduto
                {
                    Nome = apDto.Nome,
                    Preco = apDto.Preco,
                    Disponivel = true
                }).ToList();
            }

            await _produtoService.AddProdutoAsync(estabelecimentoId, produto);
            return CreatedAtAction(nameof(GetProdutoPorId), new { estabelecimentoId = estabelecimentoId, produtoId = produto.Id }, produto);
        }

       

        /// <summary>
        /// Obtém todos os produtos de um estabelecimento.
        /// </summary>
        /// <param name="estabelecimentoId">ID do estabelecimento.</param>
        /// <param name="pageNumber">Número da página (padrão: 1).</param>
        /// <param name="pageSize">Tamanho da página (padrão: 10).</param>
        /// <returns>Lista de produtos.</returns>
        [Authorize(Roles = "Proprietario,Administrador")]
        [Authorize(Policy = "GerenciarEstabelecimento")]
        [HttpGet("{estabelecimentoId:int}/produtos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProdutosDoEstabelecide(int estabelecimentoId, int pageNumber = 1, int pageSize = 10)
        {
            var paginationParameters = new PaginationParameters
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }

            // Verificar se o usuário é proprietário ou gerente do estabelecimento
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(estabelecimentoId);
            if (estabelecimento == null)
            {
                return NotFound("Estabelecimento não encontrado");
            }

            if (estabelecimento.UsuarioId != userId)
            {
                return Forbid("Usuário não autorizado a acessar este estabelecimento");
            }

            var produtos = await _produtoService.GetProdutosByEstabelecimentoIdAsync(estabelecimentoId);
            return Ok(produtos);
        }

        /// <summary>
        /// Atualiza um produto existente do estabelecimento.
        /// </summary>
        /// <param name="estabelecimentoId">ID do estabelecimento.</param>
        /// <param name="produtoId">ID do produto.</param>
        /// <param name="produtoDto">Dados atualizados do produto.</param>
        /// <returns>Status de sucesso ou erro.</returns>
        [Authorize(Roles = "Proprietario,Administrador")]
        [Authorize(Policy = "GerenciarEstabelecimento")]
        [HttpPut("{estabelecimentoId:int}/produtos/{produtoId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AtualizarProduto(int estabelecimentoId, int produtoId, [FromBody] ProdutoDto produtoDto)
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

            // Verificar se o usuário é proprietário ou gerente do estabelecimento
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(estabelecimentoId);
            if (estabelecimento == null)
            {
                return NotFound("Estabelecimento não encontrado");
            }

            if (estabelecimento.UsuarioId != userId)
            {
                return Forbid("Usuário não autorizado a acessar este estabelecimento");
            }

            var produto = await _produtoService.GetProdutoByIdAsync(estabelecimentoId, produtoId);
            if (produto == null || produto.EstabelecimentoId != estabelecimentoId)
            {
                return NotFound("Produto não encontrado ou não pertence a este estabelecimento");
            }

            produto.Nome = produtoDto.Nome;
            produto.Descricao = produtoDto.Descricao;
            produto.Preco = produtoDto.Preco;
            produto.QuantidadeEmEstoque = produtoDto.QuantidadeEmEstoque;
            produto.CategoriaId = produtoDto.CategoriaId;
            

            await _produtoService.UpdateProdutoAsync(estabelecimentoId, produto);
            return NoContent();
        }

        /// <summary>
        /// Exclui um produto do estabelecimento.
        /// </summary>
        /// <param name="estabelecimentoId">ID do estabelecimento.</param>
        /// <param name="produtoId">ID do produto.</param>
        /// <returns>Status de sucesso ou erro.</returns>
        [Authorize(Roles = "Proprietario,Administrador")]
        [Authorize(Policy = "GerenciarEstabelecimento")]
        [HttpDelete("{estabelecimentoId:int}/produtos/{produtoId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ExcluirProduto(int estabelecimentoId, int produtoId)
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }

            // Verificar se o usuário é proprietário ou gerente do estabelecimento
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(estabelecimentoId);
            if (estabelecimento == null)
            {
                return NotFound("Estabelecimento não encontrado");
            }

            if (estabelecimento.UsuarioId != userId)
            {
                return Forbid("Usuário não autorizado a acessar este estabelecimento");
            }

            var produto = await _produtoService.GetProdutoByIdAsync(estabelecimentoId, produtoId);
            if (produto == null || produto.EstabelecimentoId != estabelecimentoId)
            {
                return NotFound("Produto não encontrado ou não pertence a este estabelecimento");
            }

            await _produtoService.RemoveProdutoAsync(estabelecimentoId, produtoId);
            return NoContent();
        }

        /// <summary>
        /// Registra uma movimentação de estoque para um produto.
        /// </summary>
        /// <param name="estabelecimentoId">ID do estabelecimento.</param>
        /// <param name="produtoId">ID do produto.</param>
        /// <param name="movimentacaoDto">Dados da movimentação.</param>
        /// <returns>Status de sucesso ou erro.</returns>
        [Authorize(Roles = "Proprietario,Administrador")]
        [Authorize(Policy = "GerenciarEstabelecimento")]
        [HttpPost("{estabelecimentoId:int}/produtos/{produtoId:int}/movimentacoes")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RegistrarMovimentacaoEstoque(int estabelecimentoId, int produtoId, [FromBody] MovimentacaoEstoqueDto movimentacaoDto)
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

            // Verificar se o usuário é proprietário ou gerente do estabelecimento
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(estabelecimentoId);
            if (estabelecimento == null)
            {
                return NotFound("Estabelecimento não encontrado");
            }

            if (estabelecimento.UsuarioId != userId)
            {
                return Forbid("Usuário não autorizado a acessar este estabelecimento");
            }

            var produto = await _produtoService.GetProdutoByIdAsync(estabelecimentoId, produtoId);
            if (produto == null || produto.EstabelecimentoId != estabelecimentoId)
            {
                return NotFound("Produto não encontrado ou não pertence a este estabelecimento");
            }

            // Atualizar a quantidade em estoque
            if (movimentacaoDto.Tipo == TipoMovimentacao.Entrada)
            {
                produto.QuantidadeEmEstoque += movimentacaoDto.Quantidade;
            }
            else if (movimentacaoDto.Tipo == TipoMovimentacao.Saida)
            {
                if (produto.QuantidadeEmEstoque < movimentacaoDto.Quantidade)
                {
                    return BadRequest("Quantidade em estoque insuficiente para a movimentação.");
                }
                produto.QuantidadeEmEstoque -= movimentacaoDto.Quantidade;
            }

            await _produtoService.UpdateProdutoAsync(estabelecimentoId, produto);

            // Registrar a movimentação
            var movimentacao = new MovimentacaoEstoque
            {
                ProdutoId = produtoId,
                Quantidade = movimentacaoDto.Quantidade,
                Tipo = movimentacaoDto.Tipo,
                DataMovimentacao = DateTime.UtcNow,
                Observacao = movimentacaoDto.Observacao
            };

            await _movimentacaoEstoqueService.AddMovimentacaoAsync(movimentacao);

            return NoContent();
        }

        /// <summary>
        /// Cria um novo entregador para o estabelecimento.
        /// </summary>
        /// <param name="estabelecimentoId">ID do estabelecimento.</param>
        /// <param name="entregadorDto">Dados do entregador a ser criado.</param>
        /// <returns>Entregador criado.</returns>
        [Authorize(Roles = "Proprietario,Administrador")]
        [Authorize(Policy = "GerenciarEstabelecimento")]
        [HttpPost("{estabelecimentoId:int}/entregadores")]
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

            // Verificar se o usuário é proprietário ou gerente do estabelecimento
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(estabelecimentoId);
            if (estabelecimento == null)
            {
                return NotFound("Estabelecimento não encontrado");
            }

            if (estabelecimento.UsuarioId != userId)
            {
                return Forbid("Usuário não autorizado a acessar este estabelecimento");
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
        /// Obtém todos os entregadores de um estabelecimento.
        /// </summary>
        /// <param name="estabelecimentoId">ID do estabelecimento.</param>
        /// <returns>Lista de entregadores.</returns>
        [Authorize(Roles = "Proprietario,Administrador")]
        [Authorize(Policy = "GerenciarEstabelecimento")]
        [HttpGet("{estabelecimentoId:int}/entregadores")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEntregadores(int estabelecimentoId)
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }

            // Verificar se o usuário é proprietário ou gerente do estabelecimento
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(estabelecimentoId);
            if (estabelecimento == null)
            {
                return NotFound("Estabelecimento não encontrado");
            }

            if (estabelecimento.UsuarioId != userId)
            {
                return Forbid("Usuário não autorizado a acessar este estabelecimento");
            }

            var entregadores = await _entregadorService.GetEntregadoresByEstabelecimentoIdAsync(estabelecimentoId);
            return Ok(entregadores);
        }

        /// <summary>
        /// Atribui uma entrega a um pedido.
        /// </summary>
        /// <param name="estabelecimentoId">ID do estabelecimento.</param>
        /// <param name="pedidoId">ID do pedido.</param>
        /// <param name="dto">Dados para atribuir a entrega.</param>
        /// <returns>Entrega criada.</returns>
        [Authorize(Roles = "Proprietario,Administrador")]
        [Authorize(Policy = "GerenciarEstabelecimento")]
        [HttpPost("{estabelecimentoId:int}/pedidos/{pedidoId:int}/atribuir-entrega")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AtribuirEntrega(int estabelecimentoId, int pedidoId, [FromBody] AtribuirEntregaDto dto)
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

            // Verificar se o usuário é proprietário ou gerente do estabelecimento
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(estabelecimentoId);
            if (estabelecimento == null)
            {
                return NotFound("Estabelecimento não encontrado");
            }

            if (estabelecimento.UsuarioId != userId)
            {
                return Forbid("Usuário não autorizado a acessar este estabelecimento");
            }

            // Verificar se o pedido pertence ao estabelecimento
            var pedido = await _pedidoService.GetPedidoByIdAsync(pedidoId);
            if (pedido == null || pedido.EstabelecimentoId != estabelecimentoId)
            {
                return NotFound("Pedido não encontrado ou não pertence a este estabelecimento");
            }

            // Criar a entrega
            var entrega = new Entrega
            {
                PedidoId = pedidoId,
                EntregadorId = dto.EntregadorId,
                DataHoraSaida = DateTime.UtcNow,
                Status = StatusEntrega.EmTransito
            };

            await _entregaService.AddAsync(entrega);

            // Atualizar o status do pedido para "SaiuParaEntrega"
            await _pedidoService.AtualizarStatusPedidoAsync(pedidoId, StatusPedido.SaiuParaEntrega);

            return Ok(entrega);
        }

        /// <summary>
        /// Obtém um entregador específico de um estabelecimento.
        /// </summary>
        /// <param name="estabelecimentoId">ID do estabelecimento.</param>
        /// <param name="entregadorId">ID do entregador.</param>
        /// <returns>O entregador encontrado ou uma mensagem de erro se não for encontrado.</returns>
        [Authorize(Roles = "Proprietario,Administrador")]
        [Authorize(Policy = "GerenciarEstabelecimento")]
        [HttpGet("{estabelecimentoId:int}/entregadores/{entregadorId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEntregadorPorId(int estabelecimentoId, int entregadorId)
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }

            // Verificar se o usuário é proprietário ou gerente do estabelecimento
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(estabelecimentoId);
            if (estabelecimento == null)
            {
                return NotFound("Estabelecimento não encontrado");
            }

            if (estabelecimento.UsuarioId != userId)
            {
                return Forbid("Usuário não autorizado a acessar este estabelecimento");
            }

            var entregador = await _entregadorService.GetByIdAsync(entregadorId);
            if (entregador == null || entregador.EstabelecimentoId != estabelecimentoId)
            {
                return NotFound("Entregador não encontrado ou não pertence a este estabelecimento");
            }

            return Ok(entregador);
        }

        /// <summary>
        /// RETORNA TODOS OS ESTABELECIMENTOS ATIVOS OU OS MAIS PRÓXIMOS DE UMA LOCALIZAÇÃO ESPECÍFICA.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="retornarTodos"></param>
        /// <returns></returns>
        [HttpGet("todos-estabelecimentos-fluxo")]
        public async Task<IActionResult> Fluxo(
                         [FromQuery] double? latitude, // Alterado para nullable
                         [FromQuery] double? longitude, // Alterado para nullable
                         [FromQuery] bool retornarTodos = false)
        {
            IEnumerable<Estabelecimento> estabelecimentos;

            // Verifica se deve retornar todos os estabelecimentos
            bool deveRetornarTodos = retornarTodos ||
                                     !latitude.HasValue ||
                                     !longitude.HasValue ||
                                     latitude <= 0 ||
                                     longitude <= 0 ||
                                     latitude < -90 || latitude > 90 ||
                                     longitude < -180 || longitude > 180;

            if (deveRetornarTodos)
            {
                // Retorna todos os estabelecimentos ativos, independente da localização
                estabelecimentos = await _estabelecimentoService.GetAllActiveAsync();
            }
            else
            {
                // Retorna apenas os estabelecimentos dentro de um raio de 5 km
                estabelecimentos = await _estabelecimentoService.GetProximosAsync(latitude.Value, longitude.Value, 5);
            }

            if (!estabelecimentos.Any())
                return Ok(new { Message = "Sem estabelecimentos na área" });

            var estabelecimentosDisponiveis = new List<object>();

            foreach (var estabelecimento in estabelecimentos)
            {
                // Verificar se o estabelecimento possui raio de entrega configurado
                if (estabelecimento.RaioEntregaKm == null || estabelecimento.RaioEntregaKm <= 0)
                {
                    continue; // Ignorar estabelecimentos sem raio de entrega válido
                }

                // Verificar se está dentro da área de entrega (apenas se não estiver retornando todos)
                if (!deveRetornarTodos && !await _estabelecimentoService.EstaDentroDaAreaEntregaAsync(estabelecimento.Id, latitude.Value, longitude.Value))
                {
                    continue; // Ignorar se estiver fora do raio
                }

                var horarioUtc = DateTime.UtcNow.TimeOfDay;
                var horarioAtual = horarioUtc.Subtract(TimeSpan.FromHours(3));

                // Normaliza o TimeSpan para horas/minutos/segundos (truncar precisão)
                horarioAtual = new TimeSpan(horarioAtual.Hours, horarioAtual.Minutes, horarioAtual.Seconds);

                // Ajusta para cenários negativos
                if (horarioAtual < TimeSpan.Zero)
                {
                    horarioAtual = horarioAtual.Add(TimeSpan.FromHours(24));
                }

                if (!await _estabelecimentoService.EstaAbertoAsync(estabelecimento.Id, DateTime.UtcNow.DayOfWeek, horarioAtual))
                {
                    continue; // Ignorar estabelecimentos fechados
                }

                // Obter o cardápio (produtos)
                var produtos = await _produtoService.GetProdutosByEstabelecimentoIdAsync(estabelecimento.Id);

                // Adicionar o estabelecimento com cardápio à lista
                estabelecimentosDisponiveis.Add(new
                {
                    Estabelecimento = estabelecimento,
                    Cardapio = produtos
                });
            }

            if (!estabelecimentosDisponiveis.Any())
                return Ok(new { Message = "Nenhum estabelecimento disponível no momento" });

            return Ok(estabelecimentosDisponiveis);
        }

        /// <summary>
        /// Obtém um estabelecimento específico.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        [HttpGet("estabelecimento-especifico")]
        public async Task<IActionResult> GetEstabelecimentoPorId(
                                                                int Id,
                                                                [FromQuery] double? latitude,
                                                                [FromQuery] double? longitude)
        {
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(Id);
            if (estabelecimento == null)
            {
                return NotFound(new { Message = "Estabelecimento não encontrado" });
            }

            var horarioFuncionamento = await _horarioFuncionamentoService.GetByEstabelecimentoIdAsync(Id);

            // Verificar se o estabelecimento possui raio de entrega configurado
            if (estabelecimento.RaioEntregaKm == null || estabelecimento.RaioEntregaKm <= 0)
            {
                return Ok(new { Message = "Configurar Raio de Entrega" });
            }

            // Verificar latitude/longitude (somente se fornecidas)
            if (latitude.HasValue && longitude.HasValue)
            {
                if (!await _estabelecimentoService.EstaDentroDaAreaEntregaAsync(estabelecimento.Id, latitude.Value, longitude.Value))
                {
                    return Ok(new { Message = "Não está na área de entrega" });
                }
            }

            // Verificar se está aberto
            var horarioUtc = DateTime.UtcNow.TimeOfDay;
            var horarioAtual = horarioUtc.Subtract(TimeSpan.FromHours(3));
            horarioAtual = new TimeSpan(horarioAtual.Hours, horarioAtual.Minutes, horarioAtual.Seconds);

            if (horarioAtual < TimeSpan.Zero)
            {
                horarioAtual = horarioAtual.Add(TimeSpan.FromHours(24));
            }

            var horariosComDiaSemana = horarioFuncionamento.Select(h => new
            {
                h.Id,
                h.EstabelecimentoId,
                DiaSemana = Enum.GetName(typeof(DayOfWeek), h.DiaSemana), // Convertendo para texto
                HoraAbertura = h.HoraAbertura.ToString(@"hh\:mm"),
                HoraFechamento = h.HoraFechamento.ToString(@"hh\:mm")
            });

            return Ok(new
            {
                Estabelecimento = estabelecimento,
                HorarioFuncionamento = horariosComDiaSemana
            });
        }

        /// <summary>
        /// Obtém todos os produtos de um estabelecimento específico.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("produtos-estabelecimento-especifico")]
        public async Task<IActionResult> GetProdutosEstabelecimentoPorId(int Id)
        {
            var produtosDisponiveis = new List<object>();

            // Obter o cardápio (produtos)
            var produtos = await _produtoService.GetProdutosByEstabelecimentoIdAsync(Id);

            // Adicionar o estabelecimento com cardápio à lista
            produtosDisponiveis.Add(new
            {
                Cardapio = produtos
            });

            if (!produtosDisponiveis.Any())
                return Ok(new { Message = "Nenhum produto disponível no momento" });

            return Ok(produtosDisponiveis);
        }

        /// <summary>
        /// Obtém um produto específico de um estabelecimento.
        /// </summary>
        /// <param name="estabelecimentoId">ID do estabelecimento.</param>
        /// <param name="produtoId">ID do produto.</param>
        /// <returns>O produto encontrado ou uma mensagem de erro se não for encontrado.</returns>
        [HttpGet("{estabelecimentoId:int}/produtos/{produtoId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProdutoPorId(int estabelecimentoId, int produtoId)
        {
            // Verificar se o usuário é proprietário ou gerente do estabelecimento
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(estabelecimentoId);
            if (estabelecimento == null)
            {
                return NotFound("Estabelecimento não encontrado");
            }

            var produto = await _produtoService.GetProdutoByIdAsync(estabelecimentoId, produtoId);
            if (produto == null || produto.EstabelecimentoId != estabelecimentoId)
            {
                return NotFound("Produto não encontrado ou não pertence a este estabelecimento");
            }

            return Ok(produto);
        }
    }
}