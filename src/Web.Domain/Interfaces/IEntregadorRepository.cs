using Web.Domain.Entities;

namespace Web.Domain.Interfaces
{
    public interface IEntregadorRepository : IGenericRepository<Entregador>
    {
        Task<IEnumerable<Entregador>> GetEntregadoresByEstabelecimentoIdAsync(int estabelecimentoId);
    }
}