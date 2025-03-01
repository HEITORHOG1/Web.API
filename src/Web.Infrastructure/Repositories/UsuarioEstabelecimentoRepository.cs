using Microsoft.EntityFrameworkCore;
using Web.Domain.Entities;
using Web.Domain.Enums;
using Web.Domain.Interfaces;
using Web.Infrastructure.Data.Context;

namespace Web.Infrastructure.Repositories
{
    public class UsuarioEstabelecimentoRepository : GenericRepository<UsuarioEstabelecimento>, IUsuarioEstabelecimentoRepository
    {
        public UsuarioEstabelecimentoRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<bool> IsProprietarioAsync(string userId, int estabelecimentoId)
        {
            var vinculo = await _context.UsuariosEstabelecimentos
                .Where(v => v.UsuarioId == userId && v.EstabelecimentoId == estabelecimentoId && v.NivelAcesso == NivelAcesso.Proprietario)
                .ToListAsync();

            return vinculo.Any();
        }

        public async Task<IEnumerable<UsuarioEstabelecimento>> GetByEstabelecimentoIdAsync(int estabelecimentoId)
        {
            return await _context.UsuariosEstabelecimentos
                .Where(ue => ue.EstabelecimentoId == estabelecimentoId)
                .ToListAsync();
        }

        public async Task<UsuarioEstabelecimento> GetVinculoAsync(string userId, int estabelecimentoId)
        {
            return await _context.UsuariosEstabelecimentos
                .FirstOrDefaultAsync(ue => ue.UsuarioId == userId && ue.EstabelecimentoId == estabelecimentoId);
        }
    }
}