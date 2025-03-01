using System.ComponentModel.DataAnnotations;

namespace MarketplaceHybrid.Shared.Models
{
    public class EnderecoClienteDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Logradouro é obrigatório")]
        public string Logradouro { get; set; }

        [Required(ErrorMessage = "Numero é obrigatório")]
        public string Numero { get; set; }

        public string? Complemento { get; set; }

        [Required(ErrorMessage = "Bairro é obrigatório")]
        public string Bairro { get; set; }

        [Required(ErrorMessage = "Cidade é obrigatória")]
        public string Cidade { get; set; }

        [Required(ErrorMessage = "Estado é obrigatório")]
        public string Estado { get; set; }

        [Required(ErrorMessage = "CEP é obrigatório")]
        [RegularExpression(@"^\d{5}-\d{3}$", ErrorMessage = "CEP inválido")]
        public string CEP { get; set; }

        public bool Principal { get; set; }
    }
}