using MarketplaceHybrid.Shared.Models;
using MarketplaceHybrid.Shared.Services.Interfaces;

namespace MarketplaceHybrid.Shared.Services
{
    public class PedidoService : IPedidoService
    {
        public Task<PedidoCompleto> AddPedidoAsync(Pedido pedido)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AtualizarStatusPedidoAsync(int pedidoId, StatusPedido novoStatus)
        {
            throw new NotImplementedException();
        }

        public Task<PedidoCompleto> GetPedidoByExternalReferenceAsync(string externalReference)
        {
            throw new NotImplementedException();
        }

        public Task<PedidoCompleto> GetPedidoByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<PedidoCompleto>> GetPedidosByEstabelecimentoIdAsync(int estabelecimentoId)
        {
            throw new NotImplementedException();
        }

        public Task<List<PedidoCompleto>> GetPedidosByUsuarioIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdatePedidoAsync(PedidoCompleto pedido)
        {
            throw new NotImplementedException();
        }
    }
}
