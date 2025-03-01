namespace Web.Domain.DTOs.Categorias
{
    public class CategoriaCreateDto
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public bool Ativo { get; set; } = true;
    }
}