using FluentValidation;

namespace Web.Domain.DTOs
{
    public class CarrinhoItemDtoValidator : AbstractValidator<CarrinhoItemDto>
    {
        public CarrinhoItemDtoValidator()
        {
            RuleFor(c => c.ProdutoId)
                .GreaterThan(0).WithMessage("O ID do produto deve ser maior que zero.");

            RuleFor(c => c.Quantidade)
                .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");

            RuleFor(c => c.ValoresOpcaoProdutoIds)
                .NotNull().WithMessage("Os valores das opções do produto são obrigatórios.");

            RuleFor(c => c.AdicionalProdutoIds)
                .NotNull().WithMessage("Os adicionais do produto são obrigatórios.");

            RuleFor(c => c.Observacao)
                .MaximumLength(500).WithMessage("A observação deve ter no máximo 500 caracteres.");
        }
    }
}