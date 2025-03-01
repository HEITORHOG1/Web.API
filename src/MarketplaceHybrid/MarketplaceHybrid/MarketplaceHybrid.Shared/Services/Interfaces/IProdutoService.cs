using MarketplaceHybrid.Shared.Models;

namespace MarketplaceHybrid.Shared.Services.Interfaces
{
    public interface IProdutoService
    {
        Task<List<Cardapio>> GetProdutosByEstabelecimentoIdAsync(int estabelecimentoId);
        Task<Cardapio?> GetProdutoByIdAsync(int estabelecimentoId, int produtoId);
    }
}
