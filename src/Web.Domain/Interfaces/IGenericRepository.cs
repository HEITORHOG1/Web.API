using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;
using Web.Domain.Paginacao;

namespace Web.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);

        Task<IEnumerable<T>> GetAllAsync();

        // Task<PagedResult<T>> GetPagedAsync(PaginationParameters paginationParameters);

        Task<PagedResult<T>> GetPagedAsync(Expression<Func<T, bool>> predicate, PaginationParameters paginationParameters);

        Task AddAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(int id);

        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}