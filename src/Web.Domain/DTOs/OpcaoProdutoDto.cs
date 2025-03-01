namespace Web.Domain.DTOs
{
    public class OpcaoProdutoDto
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string Nome { get; set; }
        public bool Obrigatorio { get; set; }

        // Caso deseje incluir os valores da opção
        public List<ValorOpcaoProdutoDto> Valores { get; set; } = new List<ValorOpcaoProdutoDto>();
    }
}