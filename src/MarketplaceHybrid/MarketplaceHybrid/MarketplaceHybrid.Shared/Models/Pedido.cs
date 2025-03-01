namespace MarketplaceHybrid.Shared.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; } // ID do cliente que fez o pedido
        public int EstabelecimentoId { get; set; } // Estabelecimento que receberá o pedido
        public decimal ValorTotal { get; set; }
        public decimal TaxaEntrega { get; set; }
        public string EnderecoEntrega { get; set; }
        public StatusPedido Status { get; private set; }
        public FormaPagamento FormaPagamento { get; set; }
        public DateTime DataCriacao { get; set; }

        public string ExternalReference { get; set; }
    }
}
