using FluentValidation;

namespace Web.Domain.DTOs
{
    public class AtribuirEntregaDtoValidator : AbstractValidator<AtribuirEntregaDto>
    {
        public AtribuirEntregaDtoValidator()
        {
            RuleFor(a => a.EntregadorId)
                .GreaterThan(0).WithMessage("O ID do entregador deve ser maior que zero.");
        }
    }
}