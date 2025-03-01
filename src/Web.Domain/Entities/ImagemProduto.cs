using System.Text.Json.Serialization;

namespace Web.Domain.Entities
{
    public class ImagemProduto
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public int EstabelecimentoId { get; set; }
        public string Url { get; set; }
        public DateTime DataCadastro { get; set; }
        public bool Principal { get; set; }

        [JsonIgnore]
        public virtual Produto Produto { get; set; }

        [JsonIgnore]
        public virtual Estabelecimento Estabelecimento { get; set; }

        public ImagemProduto()
        {
            DataCadastro = DateTime.UtcNow;
            Principal = true;
        }
    }
}