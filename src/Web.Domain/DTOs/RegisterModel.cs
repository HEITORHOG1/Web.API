namespace Web.Domain.DTOs
{
    public class RegisterModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? CPF_CNPJ { get; set; }
        public string? NomeUsuario { get; set; }
        public string? Endereco { get; set; }
        public string? CEP { get; set; }
        public string? Telefone { get; set; }
        public Enums.UserRole Role { get; set; }
    }
}