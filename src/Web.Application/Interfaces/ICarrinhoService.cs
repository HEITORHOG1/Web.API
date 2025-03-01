using Web.Domain.Entities;

namespace Web.Application.Interfaces
{
    public interface ICarrinhoService
    {
        Task<IEnumerable<CarrinhoItem>> GetCarrinhoItensAsync(string usuarioId);

        Task AddItemAoCarrinhoAsync(string usuarioId, int estabelecimentoId, int produtoId, int quantidade);

        Task AtualizarQuantidadeAsync(string usuarioId, int produtoId, int quantidade);

        Task RemoverItemDoCarrinhoAsync(string usuarioId, int produtoId);

        Task LimparCarrinhoAsync(string usuarioId);

        Task<CarrinhoItem?> GetItemAsync(string userId, int produtoId, int estabelecimentoId);

        Task AtualizarItemAsync(CarrinhoItem carrinhoItem);

        Task AdicionarItensAoCarrinhoAsync(string usuarioId, List<CarrinhoItem> itens);

        Task AtualizarItensNoCarrinhoAsync(string usuarioId, List<CarrinhoItem> itens);

        Task RemoverItensDoCarrinhoAsync(string usuarioId, List<int> produtoIds);
    }
}