using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace Web.Application
{
    public class CreateEstabelecimentoDto
    {
        public string RazaoSocial { get; set; }

        public string NomeFantasia { get; set; }

  
        public string CNPJ { get; set; }

        public string Telefone { get; set; }

        public string Endereco { get; set; }

        public bool Status { get; set; }

        public string Cep { get; set; }

        public string Numero { get; set; }

        public decimal? TaxaEntregaFixa { get; set; }

        public IFormFile? UrlImagem { get; set; }

        public string Descricao { get; set; }
    }
}