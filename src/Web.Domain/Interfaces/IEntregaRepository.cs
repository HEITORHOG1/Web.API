using Web.Domain.Entities;

namespace Web.Domain.Interfaces
{
    public interface IEntregaRepository : IGenericRepository<Entrega>
    {
        Task<Entrega> GetByPedidoIdAsync(int pedidoId);

        Task<IEnumerable<Entrega>> GetEntregasByEstabelecimentoIdAsync(int estabelecimentoId);

        Task<IEnumerable<Entrega>> GetEntregasByEntregadorIdAsync(int entregadorId);
    }
}