namespace Web.Domain.Entities
{
    public class AdicionalProduto
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public bool Disponivel { get; set; }

        public virtual Produto Produto { get; set; }
    }
}