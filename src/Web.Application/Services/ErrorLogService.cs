using Web.Application.Interfaces;
using Web.Domain.Entities;
using Web.Domain.Interfaces;
using Web.Domain.Paginacao;

namespace Web.Application.Services
{
    public class ErrorLogService : IErrorLogService
    {
        private readonly IErrorLogRepository _errorLogRepository;

        public ErrorLogService(IErrorLogRepository errorLogRepository)
        {
            _errorLogRepository = errorLogRepository;
        }

        public async Task LogErrorAsync(ErrorLog errorLog)
        {
            await _errorLogRepository.AddAsync(errorLog);
        }

        public async Task<PagedResult<ErrorLog>> SearchErrorsByKeywordAsync(string keyword, PaginationParameters paginationParameters)
        {
            return await _errorLogRepository.GetByMessageAsync(keyword, paginationParameters);
        }

        public async Task<IEnumerable<ErrorLog>> GetAllErrorsOrderedByDateAsync()
        {
            return await _errorLogRepository.GetAllOrderedByDateAsync();
        }
    }
}