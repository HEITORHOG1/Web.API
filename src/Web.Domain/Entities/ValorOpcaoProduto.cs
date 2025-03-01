using System.Text.Json.Serialization;

namespace Web.Domain.Entities
{
    public class ValorOpcaoProduto
    {
        public int Id { get; set; }
        public int OpcaoProdutoId { get; set; }
        public string Descricao { get; set; }
        public decimal PrecoAdicional { get; set; } // Preço adicional se aplicável

        [JsonIgnore]
        public virtual OpcaoProduto OpcaoProduto { get; set; }
    }
}