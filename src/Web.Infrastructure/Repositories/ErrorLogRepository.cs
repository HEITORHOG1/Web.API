using Microsoft.EntityFrameworkCore;
using Web.Domain.Entities;
using Web.Domain.Interfaces;
using Web.Domain.Paginacao;
using Web.Infrastructure.Data.Context;

namespace Web.Infrastructure.Repositories
{
    public class ErrorLogRepository : GenericRepository<ErrorLog>, IErrorLogRepository
    {
        public ErrorLogRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<ErrorLog>> GetByMessageAsync(string keyword, PaginationParameters paginationParameters)
        {
            var query = _dbSet
                        .Where(e => EF.Functions.Like(e.Message, $"%{keyword}%"))
                        .OrderByDescending(e => e.DateOccurred);

            return await query.GetPagedAsync(paginationParameters.PageNumber, paginationParameters.PageSize);
        }

        public async Task<IEnumerable<ErrorLog>> GetAllOrderedByDateAsync()
        {
            return await _dbSet.OrderByDescending(e => e.DateOccurred).ToListAsync();
        }
    }
}