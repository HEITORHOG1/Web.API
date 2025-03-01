using Web.Domain.Enums;

namespace Web.Domain.Entities
{
    public class CarrinhoItem
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; }
        public int EstabelecimentoId { get; set; }
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public DateTime DataAdicionado { get; set; }
        public decimal PrecoUnitario { get; set; }
        public StatusCarrinhoItem Status { get; set; }
        public virtual Produto Produto { get; set; }
        public virtual Estabelecimento Estabelecimento { get; set; }

        public CarrinhoItem()
        {
            DataAdicionado = DateTime.UtcNow;
            Status = StatusCarrinhoItem.Ativo;
        }
    }
}