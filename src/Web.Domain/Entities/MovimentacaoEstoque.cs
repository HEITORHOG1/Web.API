using System.Text.Json.Serialization;

namespace Web.Domain.Entities
{
    public class MovimentacaoEstoque
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public int EstabelecimentoId { get; set; }
        public int Quantidade { get; set; }
        public TipoMovimentacao Tipo { get; set; }
        public DateTime DataMovimentacao { get; set; }
        public string Observacao { get; set; }

        [JsonIgnore]
        public virtual Produto Produto { get; set; }

        [JsonIgnore]
        public virtual Estabelecimento Estabelecimento { get; set; }

        public MovimentacaoEstoque()
        {
            DataMovimentacao = DateTime.UtcNow;
        }
    }

    public enum TipoMovimentacao
    {
        Entrada,
        Saida
    }
}