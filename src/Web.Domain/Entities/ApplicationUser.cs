using Microsoft.AspNetCore.Identity;

namespace Web.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? CPF_CNPJ { get; set; }
        public string? NomeUsuario { get; set; }
        public string? Endereco { get; set; }
        public string? CEP { get; set; }
        public string? Telefone { get; set; }
        public bool Ativo { get; set; } = true;
        public DateTime DataDeCadastro { get; set; } = DateTime.UtcNow;

        // Propriedades para o refresh token
        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        // Propriedades de segurança adicionais
        public DateTime? UltimoLogin { get; set; }

        public string? UltimoLoginIP { get; set; }
        public int TentativasLoginFalhas { get; set; }
        public DateTime? BloqueioExpiraEm { get; set; }
    }
}