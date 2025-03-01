namespace Web.Domain.DTOs
{
    public class UpdateUserModel
    {
        public string Id { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? CPF_CNPJ { get; set; }
        public string? NomeUsuario { get; set; }
        public string? Endereco { get; set; }
        public string? CEP { get; set; }
        public string? Telefone { get; set; }
        public bool? Ativo { get; set; }
    }
}