using Web.Domain.Entities;

namespace Web.Domain.Interfaces
{
    public interface IOpcaoProdutoRepository : IGenericRepository<OpcaoProduto>
    {
        Task<List<OpcaoProduto>> GetAllByProdutoIdAsync(int estabelecimentoId, int produtoId);

        Task<OpcaoProduto> GetOpcaoByIdAsync(int estabelecimentoId, int produtoId, int opcaoId);

        Task AddOpcaoAsync(int estabelecimentoId, int produtoId, OpcaoProduto opcao);

        Task UpdateOpcaoAsync(int estabelecimentoId, int produtoId, OpcaoProduto opcao);

        Task RemoveOpcaoAsync(int estabelecimentoId, int produtoId, int opcaoId);

        Task ReplicarOpcoesParaProdutosDaCategoriaAsync(int categoriaId, int produtoOrigemId);
    }
}