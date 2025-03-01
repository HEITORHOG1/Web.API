using Web.Domain.Enums;

namespace Web.Domain.DTOs
{
    /// <summary>
    /// Modelo para registrar um novo funcionário e vinculá-lo a um estabelecimento.
    /// </summary>
    public class RegisterFuncionarioModel
    {
        public int EstabelecimentoId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string CPF_CNPJ { get; set; }
        public string NomeUsuario { get; set; }
        public string Endereco { get; set; }
        public string CEP { get; set; }
        public string Telefone { get; set; }
        public NivelAcesso NivelAcesso { get; set; } // Enum com valores como PROPRIETARIO, GERENTE, ATENDENTE
        public string Role { get; set; } // Papel que o usuário terá (Gerente, Atendente, etc.)
    }
}