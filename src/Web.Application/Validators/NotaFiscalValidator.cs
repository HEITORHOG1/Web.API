using FluentValidation;
using Web.Domain.Entities;

namespace Web.Application.Validators
{
    public class NotaFiscalValidator : AbstractValidator<NotaFiscal>
    {
        public NotaFiscalValidator()
        {
            RuleFor(nf => nf.Numero).NotEmpty().WithMessage("Número é obrigatório.");
            RuleFor(nf => nf.DataEmissao).NotEmpty().WithMessage("Data de emissão é obrigatória.");
            RuleFor(nf => nf.Produtos).NotEmpty().WithMessage("Produtos são obrigatórios.")
                .ForEach(produto => produto
                    .SetValidator(new NotaFiscalProdutoValidator()));
        }
    }
}