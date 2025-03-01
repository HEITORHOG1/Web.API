using Microsoft.EntityFrameworkCore;
using Web.Domain.Entities;
using Web.Domain.Interfaces;
using Web.Infrastructure.Data.Context;

namespace Web.Infrastructure.Repositories
{
    public class NotaFiscalRepository : GenericRepository<NotaFiscal>, INotaFiscalRepository
    {
        private readonly AppDbContext _context;

        public NotaFiscalRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NotaFiscal>> GetAllByEstabelecimentoIdAsync(int estabelecimentoId)
        {
            return await _context.NotasFiscais
                .Where(nf => nf.EstabelecimentoId == estabelecimentoId)
                .Include(nf => nf.Produtos)
                .Include(nf => nf.Fornecedor)
                .ToListAsync();
        }

        public override async Task<NotaFiscal> GetByIdAsync(int id)
        {
            return await _context.NotasFiscais
                .Include(nf => nf.Fornecedor)
                .Include(nf => nf.Produtos)
                .FirstOrDefaultAsync(nf => nf.Id == id);
        }
    }
}