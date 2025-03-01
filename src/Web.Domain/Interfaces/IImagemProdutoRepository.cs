using Web.Domain.Entities;

namespace Web.Domain.Interfaces
{
    public interface IImagemProdutoRepository : IGenericRepository<ImagemProduto>
    {
        Task<IEnumerable<ImagemProduto>> GetImagensByProdutoIdAsync(int produtoId);

        Task<ImagemProduto> GetImagemPrincipalByProdutoIdAsync(int produtoId);
    }
}