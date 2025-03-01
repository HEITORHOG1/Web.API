using System.Text.Json.Serialization;

namespace Web.Domain.Entities
{
    public class OpcaoProduto
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string Nome { get; set; }
        public bool Obrigatorio { get; set; }

        [JsonIgnore]
        public virtual Produto Produto { get; set; }

        [JsonIgnore]
        public virtual ICollection<ValorOpcaoProduto> Valores { get; set; }

        public OpcaoProduto()
        {
            Valores = new List<ValorOpcaoProduto>();
        }
    }
}