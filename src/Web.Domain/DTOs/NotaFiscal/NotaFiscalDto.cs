namespace Web.Domain.DTOs.NotaFiscal
{
    public class NotaFiscalDto
    {
        public int Id { get; set; }
        public string Numero { get; set; }
        public DateTime DataEmissao { get; set; }
        public int EstabelecimentoId { get; set; }
        public int FornecedorId { get; set; }
        public decimal ValorTotal { get; set; }
        public List<NotaFiscalProdutoDto> Produtos { get; set; } = new List<NotaFiscalProdutoDto>();
    }
}