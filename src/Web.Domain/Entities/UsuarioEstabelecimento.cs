using Web.Domain.Enums;

namespace Web.Domain.Entities
{
    public class UsuarioEstabelecimento
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; }
        public int EstabelecimentoId { get; set; }
        public NivelAcesso NivelAcesso { get; set; }
        public DateTime DataCadastro { get; set; }
        public bool Ativo { get; set; }
        public virtual ApplicationUser Usuario { get; set; }
        public virtual Estabelecimento Estabelecimento { get; set; }

        public UsuarioEstabelecimento()
        {
            DataCadastro = DateTime.UtcNow;
            Ativo = true;
        }
    }
}