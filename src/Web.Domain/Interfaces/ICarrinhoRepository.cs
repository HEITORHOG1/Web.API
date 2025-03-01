using Web.Domain.Entities;

namespace Web.Domain.Interfaces
{
    public interface ICarrinhoRepository : IGenericRepository<CarrinhoItem>
    {
        Task<IEnumerable<CarrinhoItem>> GetCarrinhoItensByUsuarioIdAsync(string usuarioId);

        Task<CarrinhoItem> GetCarrinhoItemAsync(string usuarioId, int produtoId);

        Task<CarrinhoItem> GetItemAsync(string userId, int produtoId, int estabelecimentoId);

        Task AtualizarItemAsync(CarrinhoItem carrinhoItem);

        Task AdicionarItensAoCarrinhoAsync(string usuarioId, List<CarrinhoItem> itens);

        Task AtualizarItensNoCarrinhoAsync(string usuarioId, List<CarrinhoItem> itens);

        Task RemoverItensDoCarrinhoAsync(string usuarioId, List<int> produtoIds);
    }
}