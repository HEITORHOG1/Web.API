using System.ComponentModel.DataAnnotations;

namespace Web.Domain.DTOs
{
    public class RegisterAdminProprietarioModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string NomeUsuario { get; set; }

        [Required]
        public string CPF_CNPJ { get; set; }

        public string Telefone { get; set; }
        public string Endereco { get; set; }
        public string CEP { get; set; }
        public TipoUsuario TipoUsuario { get; set; }
    }

    public enum TipoUsuario
    {
        Administrador = 4,
        Proprietario = 2,
    }
}