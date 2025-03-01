using Web.Domain.DTOs.Categorias;
using Web.Domain.DTOs.Produtos;
using Web.Domain.Entities;

namespace Web.Application.Interfaces
{
    public interface IProdutoService
    {
        Task<Produto> GetProdutoByIdAsync(int estabelecimentoId, int id);

        Task AddProdutoAsync(int estabelecimentoId, Produto produto);

        Task UpdateProdutoAsync(int estabelecimentoId, Produto produto);

        Task RemoveProdutoAsync(int estabelecimentoId, int id);

        Task<List<ProdutoDto>> GetProdutosByEstabelecimentoIdAsync(int estabelecimentoId);

        Task EntradaProdutoAsync(int estabelecimentoId, int produtoId, int quantidade, string observacao = null);

        Task VenderProdutoAsync(int estabelecimentoId, int produtoId, int quantidade);

        Task<List<Produto>> GetProdutosByCategoriaIdAsync(int estabelecimentoId, int categoriaId);

        Task<CategoriaDto> GetCategoriaByIdAsync(int estabelecimentoId, int categoriaId); // Tipo de retorno alterado

        Task<bool> ExistsAsync(int estabelecimentoId, string nome);

        Task<List<Produto>> ProdutoCadastradoNoEstabelecimentoAsync(int estabelecimentoId, string nomeProduto);

        Task<ProdutoDto> CreateProdutoAsync(int estabelecimentoId, string userId, ProdutoCreateDto produtoDto);

        Task<List<ProdutoDto>> GetProdutosAsync(int estabelecimentoId, string userId);

        Task<ProdutoDto> GetProdutoByIdDtoAsync(int estabelecimentoId, int id, string userId);

        Task UpdateProdutoAsync(int estabelecimentoId, int id, string userId, ProdutoUpdateDto produtoDto);
    }
}