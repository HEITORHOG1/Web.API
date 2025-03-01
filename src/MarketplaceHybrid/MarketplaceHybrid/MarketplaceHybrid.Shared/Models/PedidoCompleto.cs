namespace MarketplaceHybrid.Shared.Models
{
    public class PedidoCompleto
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; }
        public int EstabelecimentoId { get; set; }
        public string EnderecoEntrega { get; set; }
        public FormaPagamento FormaPagamento { get; set; }
        public decimal TaxaEntrega { get; set; }
        public decimal ValorTotal { get; set; }
        public string ExternalReference { get; set; }
        public StatusPedido Status { get; set; }
        public DateTime DataCriacao { get; set; }
        public List<ItemPedidoCompleto> Itens { get; set; } = new List<ItemPedidoCompleto>();
    }

    public class ItemPedidoCompleto
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public string Observacao { get; set; }
    }
}
