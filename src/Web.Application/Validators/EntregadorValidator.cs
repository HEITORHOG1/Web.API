using FluentValidation;
using Web.Domain.Entities;

namespace Web.Application.Validators
{
    public class EntregadorValidator : AbstractValidator<Entregador>
    {
        public EntregadorValidator()
        {
            RuleFor(e => e.Nome)
                .NotEmpty().WithMessage("O nome é obrigatório.");

            RuleFor(e => e.Telefone)
                .NotEmpty().WithMessage("O telefone é obrigatório.");

            RuleFor(e => e.Veiculo)
                .NotEmpty().WithMessage("O veículo é obrigatório.");

            RuleFor(e => e.PlacaVeiculo)
                .NotEmpty().WithMessage("A placa do veículo é obrigatória.");

            RuleFor(e => e.Documento)
                .NotEmpty().WithMessage("O documento é obrigatório.");

            RuleFor(e => e.EstabelecimentoId)
                .GreaterThan(0).WithMessage("O ID do estabelecimento é obrigatório.");
        }
    }
}