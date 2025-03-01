namespace Web.Domain.DTOs.NotaFiscal
{
    public class NotaFiscalProdutoDto
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}