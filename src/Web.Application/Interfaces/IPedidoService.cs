using MercadoPago.Resource.Payment;
using Web.Domain.DTOs.Pedidos;
using Web.Domain.Entities;
using Web.Domain.Enums;

namespace Web.Application.Interfaces
{
    public interface IPedidoService
    {
        Task<Pedido?> FinalizarPedidoAsync(PedidoCreateDto pedidoDto, string usuarioId);

        Task<Pedido> GetPedidoByIdAsync(int id);

        Task<IEnumerable<Pedido>> GetPedidosByUsuarioIdAsync(string usuarioId);

        Task<IEnumerable<Pedido>> GetPedidosByEstabelecimentoIdAsync(int estabelecimentoId);

        Task AddPedidoAsync(Pedido pedido);

        Task UpdatePedidoAsync(Pedido pedido);

        Task UpdateStatusPedidoAsync(int pedidoId, StatusPedido novoStatus);

        Task ConfirmarPedidoAsync(int pedidoId);

        Task CancelarPedidoAsync(int pedidoId);

        Task AtualizarStatusPedidoAsync(int pedidoId, StatusPedido novoStatus);

        Task<Pedido> GetPedidoByExternalReferenceAsync(string externalReference);

        Task<Payment> ObterPagamentoDoMercadoPagoAsync(string paymentId);
    }
}