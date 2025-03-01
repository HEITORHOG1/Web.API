using Web.Domain.Entities;
using Web.Domain.Paginacao;

namespace Web.Domain.Interfaces
{
    public interface IErrorLogRepository : IGenericRepository<ErrorLog>
    {
        Task<PagedResult<ErrorLog>> GetByMessageAsync(string keyword, PaginationParameters paginationParameters);

        Task<IEnumerable<ErrorLog>> GetAllOrderedByDateAsync();
    }
}