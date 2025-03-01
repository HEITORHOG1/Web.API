using MarketplaceHybrid.Shared.Models;

namespace MarketplaceHybrid.Shared.Services.Interfaces
{
    public interface ICarrinhoService
    {
        Task<List<CarrinhoItem>> ObterCarrinhoAsync();

        Task AdicionarAoCarrinhoAsync(CarrinhoItem item);

        Task RemoverDoCarrinhoAsync(int produtoId);

        Task SalvarCarrinhoAsync(List<CarrinhoItem> carrinho);

        Task LimparCarrinhoAsync();

        void MarcarComoInicializado();
        event Func<Task> CarrinhoAtualizado;
    }
}