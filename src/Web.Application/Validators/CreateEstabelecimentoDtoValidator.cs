using FluentValidation;
using Web.Application;

namespace Web.API.Validators
{
    public class CreateEstabelecimentoDtoValidator : AbstractValidator<CreateEstabelecimentoDto>
    {
        public CreateEstabelecimentoDtoValidator()
        {
            RuleFor(e => e.NomeFantasia)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .Length(3, 255).WithMessage("O nome deve ter entre 3 e 255 caracteres.");

            RuleFor(e => e.Endereco)
                .NotEmpty().WithMessage("O endereço é obrigatório.")
                .Length(5, 500).WithMessage("O endereço deve ter entre 5 e 500 caracteres.");

            RuleFor(e => e.Telefone)
                .NotEmpty().WithMessage("O telefone é obrigatório.")
                .Matches(@"^\d{10,15}$").WithMessage("O telefone deve conter entre 10 e 15 dígitos.");

            RuleFor(e => e.CNPJ)
                .NotEmpty().WithMessage("O CNPJ é obrigatório.")
                .Matches(@"^\d{14}$").WithMessage("O CNPJ deve conter exatamente 14 dígitos.");
        }
    }
}