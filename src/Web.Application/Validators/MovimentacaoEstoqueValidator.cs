using FluentValidation;
using Web.Domain.Entities;

namespace Web.Application.Validators
{
    public class MovimentacaoEstoqueValidator : AbstractValidator<MovimentacaoEstoque>
    {
        public MovimentacaoEstoqueValidator()
        {
            RuleFor(x => x.Quantidade).GreaterThan(0).WithMessage("A quantidade deve ser maior que 0");
            RuleFor(x => x.ProdutoId).NotEmpty().WithMessage("O produto deve ser informado");
            RuleFor(x => x.Tipo).IsInEnum().WithMessage("Tipo de movimentação inválido");
        }
    }
}