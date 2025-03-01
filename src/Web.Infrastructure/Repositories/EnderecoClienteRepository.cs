using Microsoft.EntityFrameworkCore;
using Web.Domain.Entities;
using Web.Domain.Interfaces;
using Web.Infrastructure.Data.Context;

namespace Web.Infrastructure.Repositories
{
    public class EnderecoClienteRepository : GenericRepository<EnderecoCliente>, IEnderecoClienteRepository
    {
        public EnderecoClienteRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<EnderecoCliente>> GetAllByUsuarioIdAsync(string usuarioId)
        {
            return await _dbSet.Where(e => e.UsuarioId == usuarioId).ToListAsync();
        }

        public async Task<EnderecoCliente> GetPrincipalByUsuarioIdAsync(string usuarioId)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.UsuarioId == usuarioId && e.Principal == true);
        }
    }
}