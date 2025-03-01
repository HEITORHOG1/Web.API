using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;
using Web.Domain.Interfaces;
using Web.Domain.Paginacao;
using Web.Infrastructure.Data.Context;

namespace Web.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        //public virtual async Task<PagedResult<T>> GetPagedAsync(PaginationParameters paginationParameters)
        //{
        //    var query = _dbSet.AsQueryable();
        //    return await query.GetPagedAsync(paginationParameters.PageNumber, paginationParameters.PageSize);
        //}

        public virtual async Task<PagedResult<T>> GetPagedAsync(
            Expression<Func<T, bool>> predicate,
            PaginationParameters paginationParameters)
        {
            var query = _dbSet.Where(predicate);
            return await query.GetPagedAsync(paginationParameters.PageNumber, paginationParameters.PageSize);
        }

        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
    }
}