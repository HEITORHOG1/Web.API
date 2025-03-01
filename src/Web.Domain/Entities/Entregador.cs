using System.Text.Json.Serialization;

namespace Web.Domain.Entities
{
    public class Entregador
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Telefone { get; set; }
        public string Veiculo { get; set; }
        public string PlacaVeiculo { get; set; }
        public string Documento { get; set; } // CPF ou RG
        public int EstabelecimentoId { get; set; }

        // Propriedade de navegação
        [JsonIgnore]
        public virtual Estabelecimento Estabelecimento { get; set; }

        // Lista de entregas associadas ao entregador
        [JsonIgnore]
        public virtual ICollection<Entrega> Entregas { get; set; }
    }
}