using System.Text.Json.Serialization;

namespace Web.Domain.Entities
{
    public class ItemPedidoOpcao
    {
        public int Id { get; set; }
        public int ItemPedidoId { get; set; }
        public int ValorOpcaoProdutoId { get; set; }

        [JsonIgnore]
        public virtual ItemPedido ItemPedido { get; set; }

        [JsonIgnore]
        public virtual ValorOpcaoProduto ValorOpcaoProduto { get; set; }
    }
}