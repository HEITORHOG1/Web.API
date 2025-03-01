namespace MarketplaceHybrid.Shared.Models
{
    public class FinalizarCompraDto
    {
        public int EstabelecimentoId { get; set; }
        public string EnderecoEntrega { get; set; }
        public FormaPagamento FormaPagamento { get; set; }
        public decimal TaxaEntrega { get; set; }
        public decimal ValorTotal { get; set; }
        public List<PedidoItemDto> Itens { get; set; }
    }
}