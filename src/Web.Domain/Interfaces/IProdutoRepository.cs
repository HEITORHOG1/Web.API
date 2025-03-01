using Web.Domain.DTOs.Produtos;
using Web.Domain.Entities;

namespace Web.Domain.Interfaces
{
    public interface IProdutoRepository : IGenericRepository<Produto>
    {
        Task<List<ProdutoDto>> GetAllByEstabelecimentoIdAsync(int estabelecimentoId);

        Task<Produto> GetProdutoByIdAsync(int estabelecimentoId, int produtoId);

        Task<List<Produto>> GetProdutosByCategoriaIdAsync(int estabelecimentoId, int categoriaId);

        Task<Produto?> GetProdutoAdiconaisByIdAsync(int estabelecimentoId, int produtoId);

        Task AddProdutoAsync(int estabelecimentoId, Produto produto);

        Task UpdateProdutoAsync(int estabelecimentoId, Produto produto);

        Task RemoveProdutoAsync(int estabelecimentoId, int produtoId);

        Task<bool> ExistsAsync(int estabelecimentoId, string nome);

        Task<List<Produto>> ProdutoCadastradoNoEstabelecimentoAsync(int estabelecimentoId, string nomeProduto);
    }
}