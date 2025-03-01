using Web.Domain.Entities;

namespace Web.Application.Interfaces
{
    public interface IPagamentoService
    {
        Task<bool> ProcessarPagamentoAsync(Pedido pedido);
    }
}