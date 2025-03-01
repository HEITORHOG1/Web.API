using FluentValidation;
using Web.Domain.Entities;

namespace Web.Application.Validators
{
    public class CarrinhoItemValidator : AbstractValidator<CarrinhoItem>
    {
        public CarrinhoItemValidator()
        {
            RuleFor(ci => ci.UsuarioId)
                .NotEmpty().WithMessage("O usuário é obrigatório.");

            RuleFor(ci => ci.ProdutoId)
                .GreaterThan(0).WithMessage("O ID do produto é obrigatório.");

            RuleFor(ci => ci.Quantidade)
                .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");

            RuleFor(ci => ci.DataAdicionado)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("A data de adição não pode ser no futuro.");
        }
    }
}