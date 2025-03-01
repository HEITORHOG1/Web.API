namespace Web.Domain.DTOs.Fornecedor
{
    public class FornecedorUpdateDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CNPJ { get; set; }
        public string Telefone { get; set; }
        public string Endereco { get; set; }
        public string Email { get; set; }
        public int EstabelecimentoId { get; set; }
    }
}