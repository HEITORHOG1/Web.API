using Microsoft.EntityFrameworkCore;
using Web.Domain.DTOs;
using Web.Domain.Entities;
using Web.Domain.Geo;
using Web.Domain.Interfaces;
using Web.Infrastructure.Data.Context;

namespace Web.Infrastructure.Repositories
{
    public class EstabelecimentoRepository : GenericRepository<Estabelecimento>, IEstabelecimentoRepository
    {
        public EstabelecimentoRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Estabelecimento>> GetAllByUserIdAsync(string userId)
        {
            // Considerar o uso de uma junção com UsuarioEstabelecimento
            return await _context.Estabelecimentos
                .Where(e => _context.UsuariosEstabelecimentos
                    .Any(ue => ue.UsuarioId == userId && ue.EstabelecimentoId == e.Id))
                .ToListAsync();
        }

        public async Task<Estabelecimento> GetVinculoAsync(string userId, int estabelecimentoId)
        {
            return await _context.Estabelecimentos
                .FirstOrDefaultAsync(e => _context.UsuariosEstabelecimentos
                    .Any(ue => ue.UsuarioId == userId && ue.EstabelecimentoId == estabelecimentoId && e.Id == estabelecimentoId));
        }

        public async Task<IEnumerable<Estabelecimento>> GetAllByProprietarioIdAsync(string proprietarioId)
        {
            return await _context.Estabelecimentos
                .Where(e => e.UsuarioId == proprietarioId)
                .ToListAsync();
        }

        public async Task<IEnumerable<UsuarioComRoleENivelAcessoDto>> GetAllUsersByEstabelecimentoIdAsync(int estabelecimentoId)
        {
            var usuarios = await _context.UsuariosEstabelecimentos
                .Where(ue => ue.EstabelecimentoId == estabelecimentoId)
                .Select(ue => new UsuarioComRoleENivelAcessoDto
                {
                    Id = ue.Usuario.Id,
                    NomeUsuario = ue.Usuario.NomeUsuario,
                    Email = ue.Usuario.Email,
                    NivelAcesso = ue.NivelAcesso,
                    CPF_CNPJ = ue.Usuario.CPF_CNPJ,
                    Telefone = ue.Usuario.Telefone,
                    Ativo = ue.Usuario.Ativo,
                    Role = _context.UserRoles
                        .Where(ur => ur.UserId == ue.UsuarioId)
                        .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return usuarios;
        }

        public async Task UpdateAsyncNess(Estabelecimento estabelecimento)
        {
            var existingEntity = await _context.Estabelecimentos.FindAsync(estabelecimento.Id);
            estabelecimento.RaioEntregaKm = 5;
            if (existingEntity != null)
            {
                // Atualiza apenas as propriedades necessárias
                _context.Entry(existingEntity).CurrentValues.SetValues(estabelecimento);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Estabelecimento>> GetProximosAsync(double latitude, double longitude, double raioKm)
        {
            var estabelecimentos = await _context.Estabelecimentos
                .Where(e => e.Status == true)
                .ToListAsync();

            return estabelecimentos
                .Where(e => GeoHelper.CalculateDistance(e.Latitude, e.Longitude, latitude, longitude) <= raioKm)
                .ToList();
        }

        public async Task<IEnumerable<Estabelecimento>> GetAllActiveAsync()
        {
            return await _context.Estabelecimentos
                .Where(e => e.Status == true)
                .ToListAsync();
        }
    }
}