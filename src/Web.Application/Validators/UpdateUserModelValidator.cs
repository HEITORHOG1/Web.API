using FluentValidation;
using Web.Domain.DTOs;

namespace Web.API.Validators
{
    public class UpdateUserModelValidator : AbstractValidator<UpdateUserModel>
    {
        public UpdateUserModelValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("O ID do usuário é obrigatório.");
            RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Um e-mail válido é necessário.");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("O nome de usuário é obrigatório.");
            RuleFor(x => x.CPF_CNPJ).NotEmpty().WithMessage("O CPF/CNPJ é obrigatório.");
        }
    }
}