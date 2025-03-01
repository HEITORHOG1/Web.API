using System.Text.Json.Serialization;

namespace Web.Domain.Entities
{
    public class Entrega
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public int EntregadorId { get; set; }
        public DateTime DataHoraSaida { get; set; }
        public DateTime? DataHoraEntrega { get; set; }
        public StatusEntrega Status { get; set; }

        // Propriedades de navegação
        [JsonIgnore]
        public virtual Pedido Pedido { get; set; }

        [JsonIgnore]
        public virtual Entregador Entregador { get; set; }
    }

    public enum StatusEntrega
    {
        Pendente,        // Entrega ainda não atribuída ou iniciada
        EmTransito,      // Entrega em andamento
        Entregue,        // Entrega concluída
        Cancelada        // Entrega cancelada
    }
}