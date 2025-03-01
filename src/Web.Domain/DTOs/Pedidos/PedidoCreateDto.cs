using Web.Domain.Enums;

namespace Web.Domain.DTOs.Pedidos
{
    public class PedidoCreateDto
    {
        public int EstabelecimentoId { get; set; }
        public string EnderecoEntrega { get; set; }
        public FormaPagamento FormaPagamento { get; set; }
        public List<ItemPedidoDto> Itens { get; set; }
    }

    public class ItemPedidoDto
    {
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }

        public string? Observacao { get; set; }
    }
}