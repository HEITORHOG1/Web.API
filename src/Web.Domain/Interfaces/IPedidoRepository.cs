using Web.Domain.Entities;

namespace Web.Domain.Interfaces
{
    public interface IPedidoRepository : IGenericRepository<Pedido>
    {
        Task<IEnumerable<Pedido>> GetByUserIdAsync(string usuarioId);

        Task<IEnumerable<Pedido>> GetByEstabelecimentoIdAsync(int estabelecimentoId);

        Task<Pedido> GetByExternalReferenceAsync(string externalReference);

        Task AddPedidoComTransacaoAsync(Pedido pedido, IEnumerable<ItemPedido> itens);
    }
}