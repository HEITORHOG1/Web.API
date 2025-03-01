using FluentValidation;
using Web.Domain.DTOs;

namespace Web.API.Validators
{
    public class RegisterFuncionarioModelValidator : AbstractValidator<RegisterFuncionarioModel>
    {
        public RegisterFuncionarioModelValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("O nome de usuário é obrigatório.");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Um e-mail válido é obrigatório.");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6).WithMessage("A senha deve ter no mínimo 6 caracteres.");
            RuleFor(x => x.NivelAcesso).NotEmpty().WithMessage("O nível de acesso é obrigatório.");
            RuleFor(x => x.EstabelecimentoId).GreaterThan(0).WithMessage("O ID do estabelecimento deve ser maior que zero.");
            RuleFor(x => x.CPF_CNPJ)
               .NotEmpty().WithMessage("O CPF/CNPJ é obrigatório.")
               .Must(x => x.Length == 11 || x.Length == 14).WithMessage("O CPF/CNPJ deve ter 11 ou 14 caracteres.")
               .Matches(@"^\d+$").WithMessage("O CPF/CNPJ deve conter apenas números.");
            RuleFor(x => x.Role).NotEmpty().WithMessage("O papel do usuário é obrigatório.");
        }
    }
}