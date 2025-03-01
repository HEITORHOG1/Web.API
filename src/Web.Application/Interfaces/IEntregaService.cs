using Web.Domain.Entities;

namespace Web.Application.Interfaces
{
    public interface IEntregaService
    {
        Task AddAsync(Entrega entrega);

        Task<Entrega> GetByPedidoIdAsync(int pedidoId);

        Task AtualizarStatusEntregaAsync(int entregaId, StatusEntrega novoStatus);

        // Outros métodos conforme necessário
    }
}