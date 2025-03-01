using Microsoft.EntityFrameworkCore;
using Web.Domain.Entities;
using Web.Domain.Interfaces;
using Web.Infrastructure.Data.Context;

namespace Web.Infrastructure.Repositories
{
    public class CategoriaRepository : GenericRepository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Categoria>> GetAllByEstabelecimentoIdAsync(int estabelecimentoId)
        {
            return await _dbSet
              .Where(c => c.EstabelecimentoId == estabelecimentoId)
              .ToListAsync();
        }
        public async Task<Categoria?> GetByIdAsync(int id, int estabelecimentoId)
        {
            return await _dbSet
                 .FirstOrDefaultAsync(c => c.Id == id && c.EstabelecimentoId == estabelecimentoId);
        }

        public async Task<bool> ExistsAsync(int estabelecimentoId, string nome)
        {
            return await _dbSet.AnyAsync(c => c.EstabelecimentoId == estabelecimentoId && c.Nome == nome);
        }
        public async Task<IEnumerable<Categoria>> GetCategoriasByEstabelecimentoIdAsync(int estabelecimentoId)
        {
            return await _dbSet
              .Where(c => c.EstabelecimentoId == estabelecimentoId)
              .ToListAsync();
        }
    }
}