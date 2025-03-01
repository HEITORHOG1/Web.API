using FluentValidation;
using Web.Domain.Entities;

namespace Web.Application.Validators
{
    public class UsuarioEstabelecimentoValidator : AbstractValidator<UsuarioEstabelecimento>
    {
        public UsuarioEstabelecimentoValidator()
        {
            RuleFor(ue => ue.UsuarioId)
                .NotEmpty().WithMessage("O ID do usuário é obrigatório.");

            RuleFor(ue => ue.EstabelecimentoId)
                .GreaterThan(0).WithMessage("O ID do estabelecimento deve ser válido.");

            RuleFor(ue => ue.NivelAcesso)
                .IsInEnum().WithMessage("O nível de acesso deve ser um valor válido.");

            RuleFor(ue => ue.Ativo)
                .NotNull().WithMessage("O campo ativo deve ser especificado.");
        }
    }
}