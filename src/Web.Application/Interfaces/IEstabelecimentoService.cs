using Web.Domain.DTOs;
using Web.Domain.Entities;

namespace Web.Application.Interfaces
{
    public interface IEstabelecimentoService
    {
        Task<IEnumerable<Estabelecimento>> GetAllByUserIdAsync(string userId);

        Task<Estabelecimento> GetByIdAsync(int id);

        Task AddAsync(Estabelecimento estabelecimento);

        Task UpdateAsync(UpdateEstabelecimento estabelecimento);

        Task DeleteAsync(int id);

        Task<bool> IsProprietarioAsync(string userId, int estabelecimentoId);

        Task<UsuarioEstabelecimento> GetVinculoAsync(string userId, int estabelecimentoId);

        Task<Estabelecimento> AddWithUserAsync(CreateEstabelecimentoDto estabelecimento, string userId);

        Task<IEnumerable<Estabelecimento>> GetAllByProprietarioIdAsync(string proprietarioId);

        Task<IEnumerable<UsuarioComRoleENivelAcessoDto>> GetAllUsersByEstabelecimentoIdAsync(int estabelecimentoId);

        Task<IEnumerable<Estabelecimento>> GetProximosAsync(double latitude, double longitude, double raioKm);

        Task<bool> EstaDentroDaAreaEntregaAsync(int estabelecimentoId, double latitude, double longitude);

        Task<bool> EstaAbertoAsync(int estabelecimentoId, DayOfWeek diaSemana, TimeSpan horaAtual);

        Task<IEnumerable<Estabelecimento>> GetAllActiveAsync();
    }
}