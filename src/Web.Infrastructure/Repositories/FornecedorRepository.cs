using Microsoft.EntityFrameworkCore;
using Web.Domain.Entities;
using Web.Domain.Interfaces;
using Web.Infrastructure.Data.Context;

namespace Web.Infrastructure.Repositories
{
    public class FornecedorRepository : GenericRepository<Fornecedor>, IFornecedorRepository
    {
        public FornecedorRepository(AppDbContext context) : base(context)
        {
        }

        public override async Task<Fornecedor> GetByIdAsync(int id)
        {
            return await _context.Fornecedores
                .Include(f => f.Estabelecimento)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<IEnumerable<Fornecedor>> GetAllByEstabelecimentoIdAsync(int estabelecimentoId)
        {
            return await _context.Fornecedores
                .Where(f => f.EstabelecimentoId == estabelecimentoId)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int estabelecimentoId, string nome)
        {
            return await _context.Fornecedores
                .AnyAsync(c => c.EstabelecimentoId == estabelecimentoId && c.Nome == nome);
        }

        public async Task<bool> ExistsAsyncCNPJ(int estabelecimentoId, string cnpj)
        {
            return await _context.Fornecedores
                .AnyAsync(c => c.EstabelecimentoId == estabelecimentoId && c.CNPJ == cnpj);
        }
    }
}