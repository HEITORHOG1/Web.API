using System.Text.Json.Serialization;

namespace Web.Domain.Entities
{
    public class ItemPedidoAdicional
    {
        public int Id { get; set; }
        public int ItemPedidoId { get; set; }
        public int AdicionalProdutoId { get; set; }

        [JsonIgnore]
        public virtual ItemPedido ItemPedido { get; set; }

        [JsonIgnore]
        public virtual AdicionalProduto AdicionalProduto { get; set; }
    }
}