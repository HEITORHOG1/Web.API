namespace Web.Domain.DTOs
{
    public class CarrinhoDto
    {
        public string UsuarioId { get; set; }
        public int EstabelecimentoId { get; set; }
        public List<CarrinhoItemDto> Itens { get; set; }
    }
}