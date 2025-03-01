using Web.Domain.Entities;

namespace Web.Domain.Interfaces
{
    public interface IValorOpcaoProdutoRepository : IGenericRepository<ValorOpcaoProduto>
    {
        Task<List<ValorOpcaoProduto>> GetAllByOpcaoIdAsync(int estabelecimentoId, int produtoId, int opcaoId);

        Task<ValorOpcaoProduto> GetValorByIdAsync(int estabelecimentoId, int produtoId, int opcaoId, int valorId);

        Task AddValorAsync(int estabelecimentoId, int produtoId, int opcaoId, ValorOpcaoProduto valor);

        Task UpdateValorAsync(int estabelecimentoId, int produtoId, int opcaoId, ValorOpcaoProduto valor);

        Task RemoveValorAsync(int estabelecimentoId, int produtoId, int opcaoId, int valorId);
    }
}