using FluentValidation;
using Web.Domain.DTOs;

namespace Web.API.Validators
{
    public class LoginModelValidator : AbstractValidator<LoginModel>
    {
        public LoginModelValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("O nome de usuário é obrigatório.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("A senha é obrigatória.");
        }
    }
}