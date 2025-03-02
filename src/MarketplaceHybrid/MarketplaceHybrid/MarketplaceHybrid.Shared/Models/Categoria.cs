namespace MarketplaceHybrid.Shared.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public int EstabelecimentoId { get; set; }
        public bool Ativo { get; set; } = true;
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
        public int Ordem { get; set; }
    }
}
