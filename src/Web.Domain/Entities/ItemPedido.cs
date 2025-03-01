using System.Text.Json.Serialization;

namespace Web.Domain.Entities
{
    public class ItemPedido
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal? PrecoUnitario { get; set; }
        public decimal? Subtotal { get; set; }
        public string? Observacao { get; set; }

        // Propriedade de navegação para Produto
        [JsonIgnore]
        public virtual Produto Produto { get; set; }

        [JsonIgnore]
        public virtual Pedido Pedido { get; set; }

        [JsonIgnore]
        public virtual ICollection<ItemPedidoOpcao> OpcoesSelecionadas { get; set; }

        [JsonIgnore]
        public virtual ICollection<ItemPedidoAdicional> AdicionaisSelecionados { get; set; }
    }
}