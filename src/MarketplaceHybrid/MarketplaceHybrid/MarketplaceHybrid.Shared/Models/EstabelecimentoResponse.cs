using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketplaceHybrid.Shared.Models
{
    public class EstabelecimentoResponse
    {
        public Estabelecimento Estabelecimento { get; set; }
        public List<HorarioFuncionamento> HorarioFuncionamento { get; set; }
    }

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
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int RaioEntregaKm { get; set; }
        public decimal TaxaEntregaFixa { get; set; }
        public string UrlImagem { get; set; }
        public string Descricao { get; set; }

        public bool EstaAberto { get; set; }

    }

    public class HorarioFuncionamento
    {
        public int Id { get; set; }
        public int EstabelecimentoId { get; set; }
        public string DiaSemana { get; set; }
        public string HoraAbertura { get; set; }
        public string HoraFechamento { get; set; }
    }
}
