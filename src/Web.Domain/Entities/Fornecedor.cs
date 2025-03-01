using System.Text.Json.Serialization;

namespace Web.Domain.Entities
{
    public class Fornecedor
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CNPJ { get; set; }
        public string Endereco { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public bool Ativo { get; set; }

        public DateTime DataCreate { get; set; }
        public int EstabelecimentoId { get; set; } // Novo campo para associar ao estabelecimento

        [JsonIgnore]
        public virtual Estabelecimento Estabelecimento { get; set; } // Relação com Estabelecimento

        [JsonIgnore]
        public virtual ICollection<NotaFiscal> NotasFiscais { get; set; }

        public Fornecedor()
        {
            NotasFiscais = new List<NotaFiscal>();
            Ativo = true;
            DataCreate = DateTime.UtcNow;
        }
    }
}