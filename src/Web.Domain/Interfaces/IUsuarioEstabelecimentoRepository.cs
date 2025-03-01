using Web.Domain.Entities;

namespace Web.Domain.Interfaces
{
    public interface IUsuarioEstabelecimentoRepository : IGenericRepository<UsuarioEstabelecimento>
    {
        // Método para verificar se o usuário é proprietário
        Task<bool> IsProprietarioAsync(string userId, int estabelecimentoId);

        Task<IEnumerable<UsuarioEstabelecimento>> GetByEstabelecimentoIdAsync(int estabelecimentoId);

        Task<UsuarioEstabelecimento> GetVinculoAsync(string userId, int estabelecimentoId);
    }
}