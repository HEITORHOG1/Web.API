using FluentValidation;

namespace Web.Domain.DTOs
{
    public class AdicionalProdutoDtoValidator : AbstractValidator<AdicionalProdutoDto>
    {
        public AdicionalProdutoDtoValidator()
        {
            RuleFor(a => a.Nome)
                .NotEmpty().WithMessage("O nome do adicional é obrigatório.")
                .MaximumLength(100).WithMessage("O nome do adicional deve ter no máximo 100 caracteres.");

            RuleFor(a => a.Preco)
                .GreaterThanOrEqualTo(0.00m).WithMessage("O preço do adicional deve ser maior ou igual a zero.");
        }
    }
}