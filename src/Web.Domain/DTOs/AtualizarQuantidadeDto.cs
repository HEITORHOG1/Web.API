namespace Web.Domain.DTOs
{
    public class AtualizarQuantidadeDto
    {
        public int ProdutoId { get; set; }
        public int EstabelecimentoId { get; set; }
        public int Quantidade { get; set; } // Pode ser positivo ou negativo
    }
}