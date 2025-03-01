using Web.Domain.Entities;
using Web.Domain.Paginacao;

namespace Web.Domain.Interfaces
{
    public interface IMovimentacaoEstoqueRepository : IGenericRepository<MovimentacaoEstoque>
    {
        Task<PagedResult<MovimentacaoEstoque>> GetByProdutoIdAsync(int produtoId, PaginationParameters paginationParameters);

        Task<MovimentacaoEstoque> GetUltimaMovimentacaoAsync(int produtoId);
    }
}