using FluentValidation;
using Web.Domain.Entities;

namespace Web.Application.Validators
{
    public class NotaFiscalProdutoValidator : AbstractValidator<NotaFiscalProduto>
    {
        public NotaFiscalProdutoValidator()
        {
            RuleFor(nfp => nfp.ProdutoId).GreaterThan(0).WithMessage("Produto ID é obrigatório.");
            RuleFor(nfp => nfp.Quantidade).GreaterThan(0).WithMessage("Quantidade deve ser maior que zero.");
            RuleFor(nfp => nfp.PrecoUnitario).GreaterThan(0).WithMessage("Preço unitário deve ser maior que zero.");
        }
    }
}