using Web.Domain.Entities;
using Web.Domain.Paginacao;

namespace Web.Application.Interfaces
{
    public interface IErrorLogService
    {
        Task LogErrorAsync(ErrorLog errorLog);

        Task<PagedResult<ErrorLog>> SearchErrorsByKeywordAsync(string keyword, PaginationParameters paginationParameters);

        Task<IEnumerable<ErrorLog>> GetAllErrorsOrderedByDateAsync();
    }
}