using System.Text.Json.Serialization;

namespace Web.Domain.Entities
{
    public class Estabelecimento
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeFantasia { get; set; }
        public string CNPJ { get; set; }
        public string Telefone { get; set; }
        public string Endereco { get; set; }
        public bool Status { get; set; }
        public string Cep { get; set; }
        public string Numero { get; set; }
        public DateTime DataCadastro { get; set; }
        public double Latitude { get; set; } // Coordenada geográfica
        public double Longitude { get; set; } // Coordenada geográfica
        public double? RaioEntregaKm { get; set; } // Raio da área de entrega em km
        public decimal? TaxaEntregaFixa { get; set; }
        public string? UrlImagem { get; set; }
        public string? Descricao { get; set; }

        [JsonIgnore]
        public virtual ICollection<HorarioFuncionamento> HorariosFuncionamento { get; set; } // Relacionamento

        [JsonIgnore]
        public virtual ICollection<Categoria> Categorias { get; set; }

        [JsonIgnore]
        public virtual ICollection<Produto> Produtos { get; set; }

        [JsonIgnore]
        public virtual ICollection<Pedido> Pedidos { get; set; }

        [JsonIgnore]
        public virtual ICollection<NotaFiscal> NotasFiscais { get; set; }

        [JsonIgnore]
        public virtual ICollection<Fornecedor> Fornecedores { get; set; }

        public Estabelecimento()
        {
            DataCadastro = DateTime.UtcNow;
            Status = true;
            Categorias = new List<Categoria>();
            Produtos = new List<Produto>();
            Pedidos = new List<Pedido>();
            NotasFiscais = new List<NotaFiscal>();
            Fornecedores = new List<Fornecedor>();
            HorariosFuncionamento = new List<HorarioFuncionamento>();
        }
    }
}