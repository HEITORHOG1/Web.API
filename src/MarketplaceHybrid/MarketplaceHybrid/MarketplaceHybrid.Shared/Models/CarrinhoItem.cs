namespace MarketplaceHybrid.Shared.Models
{
    public class CarrinhoItem
    {
        public int ProdutoId { get; set; }
        public string Nome { get; set; }
        public int Quantidade { get; set; }
        public decimal Preco { get; set; } // Substitui PrecoUnitario
        public decimal Total => Quantidade * Preco; // Calculado automaticamente
        public int EstabelecimentoId { get; set; } // Necessário para vincular ao estabelecimento
    }
}