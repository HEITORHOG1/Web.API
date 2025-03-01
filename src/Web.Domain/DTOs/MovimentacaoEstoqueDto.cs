using Web.Domain.Entities;

namespace Web.Domain.DTOs
{
    public class MovimentacaoEstoqueDto
    {
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public TipoMovimentacao Tipo { get; set; }
        public string Observacao { get; set; }
    }
}