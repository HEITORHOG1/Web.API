using FluentValidation;
using Web.Domain.DTOs;

namespace Web.API.Validators
{
    public class ResetPasswordModelValidator : AbstractValidator<ResetPasswordModel>
    {
        public ResetPasswordModelValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Um e-mail válido é obrigatório.");
            RuleFor(x => x.Token).NotEmpty().WithMessage("O token de redefinição é obrigatório.");
            RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(6).WithMessage("A nova senha deve ter no mínimo 6 caracteres.");
        }
    }
}