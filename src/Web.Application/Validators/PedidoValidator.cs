using FluentValidation;
using Web.Domain.Entities;

namespace Web.Application.Validators
{
    public class PedidoValidator : AbstractValidator<Pedido>
    {
        public PedidoValidator()
        {
            RuleFor(p => p.UsuarioId)
                .NotEmpty().WithMessage("O usuário é obrigatório.");

            RuleFor(p => p.EstabelecimentoId)
                .GreaterThan(0).WithMessage("O ID do estabelecimento é obrigatório.");

            RuleFor(p => p.EnderecoEntrega)
                .NotEmpty().WithMessage("O endereço de entrega é obrigatório.")
                .MaximumLength(250).WithMessage("O endereço de entrega deve ter no máximo 250 caracteres.");

            RuleFor(p => p.FormaPagamento)
                .IsInEnum().WithMessage("A forma de pagamento é inválida.");

            RuleFor(p => p.Itens)
                .NotEmpty().WithMessage("O pedido deve ter pelo menos um item.");

            RuleFor(p => p.ValorTotal)
                .GreaterThan(0).WithMessage("O valor total do pedido deve ser maior que zero.");

            RuleForEach(p => p.Itens)
                .SetValidator(new ItemPedidoValidator());

            RuleFor(p => p.DataCriacao)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("A data de criação não pode ser no futuro.");
        }
    }
}