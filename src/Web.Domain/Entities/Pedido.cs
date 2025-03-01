using System.Text.Json.Serialization;
using Web.Domain.Enums;

namespace Web.Domain.Entities
{
    public class Pedido
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; } // ID do cliente que fez o pedido
        public int EstabelecimentoId { get; set; } // Estabelecimento que receberá o pedido
        public decimal ValorTotal { get; set; }
        public decimal TaxaEntrega { get; set; }
        public string EnderecoEntrega { get; set; }
        public StatusPedido Status { get; private set; }
        public FormaPagamento FormaPagamento { get; set; }
        public DateTime DataCriacao { get; set; }

        public string ExternalReference { get; set; }

        [JsonIgnore]
        public virtual Estabelecimento Estabelecimento { get; set; }

        [JsonIgnore]
        public virtual ICollection<ItemPedido> Itens { get; set; }

        [JsonIgnore]
        public virtual Entrega Entrega { get; set; }

        // Método para atualizar o status
        public void AtualizarStatus(StatusPedido novoStatus)
        {
            if (PodeAtualizarStatus(novoStatus))
            {
                Status = novoStatus;
            }
            else
            {
                throw new Exception($"Transição de status de {Status} para {novoStatus} não é permitida.");
            }
        }

        private bool PodeAtualizarStatus(StatusPedido novoStatus)
        {
            // Defina as transições permitidas
            return novoStatus switch
            {
                StatusPedido.AguardandoPagamento => Status == StatusPedido.Criado,
                StatusPedido.PagamentoAprovado => Status == StatusPedido.AguardandoPagamento,
                StatusPedido.Confirmado => Status == StatusPedido.PagamentoAprovado,
                StatusPedido.EmPreparo => Status == StatusPedido.Confirmado,
                StatusPedido.Pronto => Status == StatusPedido.EmPreparo,
                StatusPedido.SaiuParaEntrega => Status == StatusPedido.Pronto,
                StatusPedido.Entregue => Status == StatusPedido.SaiuParaEntrega,
                StatusPedido.Cancelado => Status == StatusPedido.Criado || Status == StatusPedido.AguardandoPagamento,
                _ => false,
            };
        }

        public Pedido()
        {
            DataCriacao = DateTime.UtcNow.AddHours(-3);
            Itens = new List<ItemPedido>();
            Status = StatusPedido.Criado;
        }
    }
}