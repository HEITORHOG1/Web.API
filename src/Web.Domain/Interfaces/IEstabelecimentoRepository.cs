using Web.Domain.DTOs;
using Web.Domain.Entities;

namespace Web.Domain.Interfaces
{
    public interface IEstabelecimentoRepository : IGenericRepository<Estabelecimento>
    {
        Task<IEnumerable<Estabelecimento>> GetAllByUserIdAsync(string userId);

        Task<Estabelecimento> GetVinculoAsync(string userId, int estabelecimentoId);

        Task<IEnumerable<Estabelecimento>> GetAllByProprietarioIdAsync(string proprietarioId);

        Task<IEnumerable<UsuarioComRoleENivelAcessoDto>> GetAllUsersByEstabelecimentoIdAsync(int estabelecimentoId);

        Task UpdateAsyncNess(Estabelecimento estabelecimento);

        Task<IEnumerable<Estabelecimento>> GetProximosAsync(double latitude, double longitude, double raioKm);

        Task<IEnumerable<Estabelecimento>> GetAllActiveAsync();
    }
}