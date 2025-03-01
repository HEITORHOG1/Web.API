using FluentValidation;
using Web.Domain.DTOs;

namespace Web.Application.Validators
{
    public class ChangePasswordModelValidator : AbstractValidator<ChangePasswordModel>
    {
        public ChangePasswordModelValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("O ID do usuário é obrigatório.");
            RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage("A senha atual é obrigatória.");
            RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(6).WithMessage("A nova senha deve ter no mínimo 6 caracteres.");
        }
    }
}