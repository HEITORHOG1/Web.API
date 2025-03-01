using Microsoft.EntityFrameworkCore;
using Web.Domain.Entities;
using Web.Domain.Interfaces;
using Web.Infrastructure.Data.Context;

namespace Web.Infrastructure.Repositories
{
    public class EntregaRepository : GenericRepository<Entrega>, IEntregaRepository
    {
        public EntregaRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Entrega?> GetByPedidoIdAsync(int pedidoId)
        {
            return await _dbSet
                .Include(e => e.Pedido)
                .Include(e => e.Entregador)
                .FirstOrDefaultAsync(e => e.PedidoId == pedidoId);
        }

        public async Task<IEnumerable<Entrega>> GetEntregasByEstabelecimentoIdAsync(int estabelecimentoId)
        {
            return await _dbSet
                .Include(e => e.Pedido)
                .Include(e => e.Entregador)
                .Where(e => e.Pedido.EstabelecimentoId == estabelecimentoId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Entrega>> GetEntregasByEntregadorIdAsync(int entregadorId)
        {
            return await _dbSet
                .Include(e => e.Pedido)
                .Where(e => e.EntregadorId == entregadorId)
                .ToListAsync();
        }
    }
}