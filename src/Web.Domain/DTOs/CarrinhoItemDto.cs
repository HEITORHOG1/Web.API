namespace Web.Domain.DTOs
{
    public class CarrinhoItemDto
    {
        public int EstabelecimentoId { get; set; }
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public List<int>? ValoresOpcaoProdutoIds { get; set; } // Tornar opcional
        public List<int>? AdicionalProdutoIds { get; set; } // Tornar opcional
        public string? Observacao { get; set; } // Tornar opcional
    }
}