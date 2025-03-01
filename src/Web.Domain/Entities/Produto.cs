using System.Text.Json.Serialization;
using Web.Domain.Enums;

namespace Web.Domain.Entities
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
        public string Imagem { get; set; }
        public bool Disponivel { get; set; }
        public int CategoriaId { get; set; }
        public int EstabelecimentoId { get; set; }
        public DateTime DataCadastro { get; set; }
        public int QuantidadeEmEstoque { get; set; }
        public int QuantidadeReservada { get; set; }
        public string? CodigoDeBarras { get; set; }
        public StatusProduto? Status { get; set; }

        [JsonIgnore]
        public virtual ICollection<OpcaoProduto> Opcoes { get; set; }

        [JsonIgnore]
        public virtual ICollection<AdicionalProduto> Adicionais { get; set; }

        // Propriedades de navegação adicionadas
        [JsonIgnore]
        public virtual Categoria Categoria { get; set; }

        [JsonIgnore]
        public virtual Estabelecimento Estabelecimento { get; set; }

        [JsonIgnore]
        public virtual ICollection<ImagemProduto> Imagens { get; set; }

        public Produto()
        {
            DataCadastro = DateTime.UtcNow;
            Disponivel = true;
            QuantidadeEmEstoque = 0;
            Opcoes = new List<OpcaoProduto>();
            Adicionais = new List<AdicionalProduto>();
            Status = StatusProduto.Ativo;
        }
    }
}