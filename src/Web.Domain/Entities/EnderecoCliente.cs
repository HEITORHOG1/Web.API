namespace Web.Domain.Entities
{
    public class EnderecoCliente
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string? Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string CEP { get; set; }
        public bool Principal { get; set; }
        public DateTime DataCadastro { get; set; }
        public int EstabelecimentoId { get; set; }

        public EnderecoCliente()
        {
            DataCadastro = DateTime.UtcNow;
            Principal = false;
        }

        public virtual ApplicationUser Usuario { get; set; }
    }
}