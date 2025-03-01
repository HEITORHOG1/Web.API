using Web.Domain.Enums;

namespace Web.Domain.DTOs.Pedidos
{
    public class PedidoUpdateDto
    {
        public int Id { get; set; }
        public StatusPedido Status { get; set; }
    }
}