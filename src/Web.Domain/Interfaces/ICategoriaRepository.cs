using Web.Domain.Entities;

namespace Web.Domain.Interfaces
{
    public interface ICategoriaRepository : IGenericRepository<Categoria>
    {
        Task<IEnumerable<Categoria>> GetAllByEstabelecimentoIdAsync(int estabelecimentoId);
        Task<Categoria?> GetByIdAsync(int id, int estabelecimentoId);
        Task<bool> ExistsAsync(int estabelecimentoId, string nome);
        Task<IEnumerable<Categoria>> GetCategoriasByEstabelecimentoIdAsync(int estabelecimentoId);
    }
}