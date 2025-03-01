using Web.Domain.Enums;

namespace Web.Domain.DTOs.Produtos
{
    public class ProdutoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
        public string Imagem { get; set; }
        public bool Disponivel { get; set; }
        public int CategoriaId { get; set; }
        public string NomeCategoria { get; set; }
        public int EstabelecimentoId { get; set; }
        public DateTime DataCadastro { get; set; }
        public int QuantidadeEmEstoque { get; set; }
        public int QuantidadeReservada { get; set; }
        public string? CodigoDeBarras { get; set; }
        public StatusProduto? Status { get; set; }

        public List<OpcaoProdutoDto> Opcoes { get; set; }
        public List<AdicionalProdutoDto> Adicionais { get; set; }
    }
}