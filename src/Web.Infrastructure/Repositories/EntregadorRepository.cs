using Microsoft.EntityFrameworkCore;
using Web.Domain.Entities;
using Web.Domain.Interfaces;
using Web.Infrastructure.Data.Context;

namespace Web.Infrastructure.Repositories
{
    public class EntregadorRepository : GenericRepository<Entregador>, IEntregadorRepository
    {
        public EntregadorRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Entregador>> GetEntregadoresByEstabelecimentoIdAsync(int estabelecimentoId)
        {
            return await _dbSet
                .Where(e => e.EstabelecimentoId == estabelecimentoId)
                .ToListAsync();
        }
    }
}