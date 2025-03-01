using FluentValidation;
using Web.Domain.DTOs;

namespace Web.Application.Validators
{
    public class ForgotPasswordModelValidator : AbstractValidator<ForgotPasswordModel>
    {
        public ForgotPasswordModelValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Um e-mail válido é obrigatório.");
        }
    }
}