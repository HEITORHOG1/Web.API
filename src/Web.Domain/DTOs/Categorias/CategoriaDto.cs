namespace Web.Domain.DTOs.Categorias
{
    public class CategoriaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public bool Ativo { get; set; }
        public int EstabelecimentoId { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}