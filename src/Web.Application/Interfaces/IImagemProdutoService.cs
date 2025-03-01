using Web.Domain.Entities;

namespace Web.Application.Interfaces
{
    public interface IImagemProdutoService
    {
        Task AddImagemProdutoAsync(ImagemProduto categoria);

        Task UpdateImagemProdutoAsync(ImagemProduto categoria);

        Task DeleteImagemProdutoAsync(int id);

        Task<ImagemProduto> GetImagemProdutoByIdAsync(int id, int produtoId, int estabelecimentoId);
    }
}