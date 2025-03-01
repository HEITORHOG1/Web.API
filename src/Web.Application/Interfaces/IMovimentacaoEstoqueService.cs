using System.Linq.Expressions;
using Web.Domain.Entities;
using Web.Domain.Paginacao;

namespace Web.Application.Interfaces
{
    public interface IMovimentacaoEstoqueService
    {
        Task<MovimentacaoEstoque> GetMovimentacaoByIdAsync(int id);

        Task<PagedResult<MovimentacaoEstoque>> GetAllMovimentacoesAsync(
                            PaginationParameters paginationParameters,
                            Expression<Func<MovimentacaoEstoque, bool>> predicate = null);

        Task<PagedResult<MovimentacaoEstoque>> GetMovimentacoesByProdutoIdAsync(int produtoId, PaginationParameters paginationParameters);

        Task AddMovimentacaoAsync(MovimentacaoEstoque movimentacao);

        Task<MovimentacaoEstoque> GetUltimaMovimentacaoAsync(int produtoId);

        Task RegistrarMovimentacaoAsync(MovimentacaoEstoque movimentacao);

        Task<MovimentacaoEstoque> GetByIdAsync(int id);
    }
}