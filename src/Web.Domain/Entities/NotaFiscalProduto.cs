using System.Text.Json.Serialization;

namespace Web.Domain.Entities
{
    public class NotaFiscalProduto
    {
        public int Id { get; set; }
        public int NotaFiscalId { get; set; }
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }

        [JsonIgnore]
        public virtual NotaFiscal NotaFiscal { get; set; }

        [JsonIgnore]
        public virtual Produto Produto { get; set; }
    }
}