using Web.Domain.Entities;

namespace Web.Application.Interfaces
{
    public interface IUsuarioEstabelecimentoService
    {
        Task<IEnumerable<UsuarioEstabelecimento>> GetAllAsync();

        Task<UsuarioEstabelecimento> GetByIdAsync(int id);

        Task AddAsync(UsuarioEstabelecimento usuarioEstabelecimento);

        Task UpdateAsync(UsuarioEstabelecimento usuarioEstabelecimento);

        Task DeleteAsync(int id);

        // Novo método para verificar se o usuário é proprietário de um estabelecimento
        Task<bool> IsProprietarioAsync(string userId, int estabelecimentoId);

        Task<IEnumerable<UsuarioEstabelecimento>> GetByEstabelecimentoIdAsync(int estabelecimentoId);

        Task<UsuarioEstabelecimento> GetVinculoAsync(string userId, int estabelecimentoId);
    }
}