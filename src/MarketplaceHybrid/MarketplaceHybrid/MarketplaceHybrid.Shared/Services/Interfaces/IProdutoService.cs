using MarketplaceHybrid.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketplaceHybrid.Shared.Services.Interfaces
{
    public interface IProdutoService
    {
        Task<List<Cardapio>> GetProdutosByEstabelecimentoIdAsync(int estabelecimentoId);
        Task<Cardapio?> GetProdutoByIdAsync(int estabelecimentoId, int produtoId);
    }
}
