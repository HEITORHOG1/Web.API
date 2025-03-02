using MarketplaceHybrid.Shared.Models;

namespace MarketplaceHybrid.Shared.Services.Interfaces
{
    public interface ICategoriaService
    {
        Task<List<Categoria>> GetCategoriasByEstabelecimentoIdAsync(int estabelecimentoId);
        Task<Categoria> GetCategoriaByIdAsync(int categoriaId);
    }
}
