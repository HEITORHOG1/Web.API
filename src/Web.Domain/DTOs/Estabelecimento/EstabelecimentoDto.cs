namespace Web.Domain.DTOs.Estabelecimento
{
    public class EstabelecimentoDto
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeFantasia { get; set; }
        public string CNPJ { get; set; }
        public string Telefone { get; set; }
        public string Endereco { get; set; }
        public string HorarioFuncionamento { get; set; }
        public bool Status { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}