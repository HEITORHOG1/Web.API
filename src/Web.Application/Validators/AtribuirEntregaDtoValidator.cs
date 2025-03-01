using FluentValidation;
using Web.Domain.DTOs;

namespace Web.Application.Validators
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