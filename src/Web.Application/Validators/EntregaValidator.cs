using FluentValidation;
using Web.Domain.Entities;

namespace Web.Application.Validators
{
    public class EntregaValidator : AbstractValidator<Entrega>
    {
        public EntregaValidator()
        {
            RuleFor(e => e.PedidoId)
                .GreaterThan(0).WithMessage("O ID do pedido é obrigatório.");

            RuleFor(e => e.EntregadorId)
                .GreaterThan(0).WithMessage("O ID do entregador é obrigatório.");

            RuleFor(e => e.DataHoraSaida)
                .NotEmpty().WithMessage("A data e hora de saída são obrigatórias.")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("A data e hora de saída não podem ser no futuro.");

            RuleFor(e => e.DataHoraEntrega)
                .GreaterThanOrEqualTo(e => e.DataHoraSaida).When(e => e.DataHoraEntrega.HasValue)
                .WithMessage("A data e hora de entrega devem ser após a data e hora de saída.");

            RuleFor(e => e.Status)
                .IsInEnum().WithMessage("O status de entrega é inválido.");
        }
    }
}