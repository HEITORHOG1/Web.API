using MarketplaceHybrid.Shared.Models;

namespace MarketplaceHybrid.Shared.Services.Interfaces
{
    public interface IPedidoService
    {
        Task<PedidoCompleto> GetPedidoByIdAsync(int id);
        Task<PedidoCompleto> GetPedidoByExternalReferenceAsync(string externalReference);
        Task<List<PedidoCompleto>> GetPedidosByUsuarioIdAsync(string userId);
        Task<List<PedidoCompleto>> GetPedidosByEstabelecimentoIdAsync(int estabelecimentoId);
        Task<PedidoCompleto> AddPedidoAsync(Pedido pedido);
        Task<bool> UpdatePedidoAsync(PedidoCompleto pedido);
        Task<bool> AtualizarStatusPedidoAsync(int pedidoId, StatusPedido novoStatus);
    }
}
