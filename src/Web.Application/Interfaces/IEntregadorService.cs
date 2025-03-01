using Web.Domain.Entities;

namespace Web.Application.Interfaces
{
    public interface IEntregadorService
    {
        Task AddAsync(Entregador entregador);

        Task<Entregador> GetByIdAsync(int entregadorId);

        Task<IEnumerable<Entregador>> GetEntregadoresByEstabelecimentoIdAsync(int estabelecimentoId);

        // Outros métodos conforme necessário (Update, Delete, etc.)
    }
}