using System.Text.Json.Serialization;

namespace Web.Domain.Entities
{
    public class NotaFiscal
    {
        public int Id { get; set; }
        public string Numero { get; set; }
        public DateTime DataEmissao { get; set; }
        public int EstabelecimentoId { get; set; }
        public int FornecedorId { get; set; }
        public decimal ValorTotal { get; set; }

        public string IdUsuario { get; set; }

        [JsonIgnore]
        public virtual Fornecedor Fornecedor { get; set; }

        [JsonIgnore]
        public virtual Estabelecimento Estabelecimento { get; set; }

        [JsonIgnore]
        public virtual ICollection<NotaFiscalProduto> Produtos { get; set; }

        public NotaFiscal()
        {
            DataEmissao = DateTime.UtcNow;
            Produtos = new List<NotaFiscalProduto>();
        }
    }
}