using System.Linq.Expressions;
using Web.Application.Interfaces;
using Web.Domain.Entities;
using Web.Domain.Interfaces;
using Web.Domain.Paginacao;

namespace Web.Application.Services
{
    public class MovimentacaoEstoqueService : IMovimentacaoEstoqueService
    {
        private readonly IMovimentacaoEstoqueRepository _repository;
        private readonly IProdutoRepository _produtoRepository;

        public MovimentacaoEstoqueService(IMovimentacaoEstoqueRepository repository, IProdutoRepository produtoRepository)
        {
            _repository = repository;
            _produtoRepository = produtoRepository;
        }

        public async Task AddMovimentacaoAsync(MovimentacaoEstoque movimentacao)
        {
            await _repository.AddAsync(movimentacao);
        }

        public async Task<PagedResult<MovimentacaoEstoque>> GetAllMovimentacoesAsync(
                            PaginationParameters paginationParameters,
                            Expression<Func<MovimentacaoEstoque, bool>> predicate = null)
        {
            predicate ??= _ => true;
            return await _repository.GetPagedAsync(predicate, paginationParameters);
        }

        public async Task<PagedResult<MovimentacaoEstoque>> GetMovimentacoesByProdutoIdAsync(int produtoId, PaginationParameters paginationParameters)
        {
            return await _repository.GetByProdutoIdAsync(produtoId, paginationParameters);
        }

        public async Task<MovimentacaoEstoque> GetMovimentacaoByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<MovimentacaoEstoque> GetUltimaMovimentacaoAsync(int produtoId)
        {
            return await _repository.GetUltimaMovimentacaoAsync(produtoId);
        }

        public async Task RegistrarMovimentacaoAsync(MovimentacaoEstoque movimentacao)
        {
            // Implementação do método
            await _repository.AddAsync(movimentacao);

            // Atualizar o estoque do produto
            var produto = await _produtoRepository.GetByIdAsync(movimentacao.ProdutoId);
            if (produto == null)
            {
                throw new Exception("Produto não encontrado.");
            }

            if (movimentacao.Tipo == TipoMovimentacao.Entrada)
            {
                produto.QuantidadeEmEstoque += movimentacao.Quantidade;
            }
            else if (movimentacao.Tipo == TipoMovimentacao.Saida)
            {
                if (produto.QuantidadeEmEstoque < movimentacao.Quantidade)
                {
                    throw new Exception("Quantidade insuficiente em estoque.");
                }
                produto.QuantidadeEmEstoque -= movimentacao.Quantidade;
            }

            await _produtoRepository.UpdateAsync(produto);
        }

        public async Task<MovimentacaoEstoque> GetByIdAsync(int id)
        {
            var movimentacao = await _repository.GetByIdAsync(id);
            if (movimentacao != null)
            {
                return movimentacao;
            }
            else
            {
                throw new Exception("Movimentação não encontrada.");
            }
        }
    }
}