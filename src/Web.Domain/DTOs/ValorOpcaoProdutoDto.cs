namespace Web.Domain.DTOs
{
    public class ValorOpcaoProdutoDto
    {
        public int Id { get; set; }
        public int OpcaoProdutoId { get; set; }
        public string Descricao { get; set; }
        public decimal PrecoAdicional { get; set; }
    }
}