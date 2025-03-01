using FluentValidation;
using Web.Domain.Entities;

namespace Web.Application.Validators
{
    public class ItemPedidoValidator : AbstractValidator<ItemPedido>
    {
        public ItemPedidoValidator()
        {
            RuleFor(i => i.ProdutoId)
                .GreaterThan(0).WithMessage("O ID do produto é obrigatório.");

            RuleFor(i => i.Quantidade)
                .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");

            RuleFor(i => i.PrecoUnitario)
                .GreaterThan(0).WithMessage("O preço unitário deve ser maior que zero.");

            RuleFor(i => i.Subtotal)
                .Equal(i => i.PrecoUnitario * i.Quantidade)
                .WithMessage("O subtotal deve ser igual ao preço unitário multiplicado pela quantidade.");

            RuleFor(i => i.Observacao)
                .MaximumLength(500).WithMessage("A observação deve ter no máximo 500 caracteres.")
                .When(i => !string.IsNullOrEmpty(i.Observacao));
        }
    }
}