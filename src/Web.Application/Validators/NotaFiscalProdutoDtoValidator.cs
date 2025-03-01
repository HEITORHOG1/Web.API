using FluentValidation;
using Web.Domain.DTOs.NotaFiscal;

namespace Web.API.Validators
{
    public class NotaFiscalProdutoDtoValidator : AbstractValidator<NotaFiscalProdutoDto>
    {
        public NotaFiscalProdutoDtoValidator()
        {
            RuleFor(np => np.ProdutoId)
                .GreaterThan(0).WithMessage("O ID do produto deve ser maior que zero.");

            RuleFor(np => np.Quantidade)
                .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");

            RuleFor(np => np.PrecoUnitario)
                .GreaterThanOrEqualTo(0.01m).WithMessage("O preço unitário deve ser maior ou igual a 0,01.");
        }
    }
}