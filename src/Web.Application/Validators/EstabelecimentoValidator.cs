using FluentValidation;
using Web.Domain.Entities;

namespace Web.Application.Validators
{
    public class EstabelecimentoValidator : AbstractValidator<Estabelecimento>
    {
        public EstabelecimentoValidator()
        {
            RuleFor(e => e.RazaoSocial)
                .NotEmpty().WithMessage("A razão social é obrigatória.")
                .Length(3, 100).WithMessage("A razão social deve ter entre 3 e 100 caracteres.");

            RuleFor(e => e.NomeFantasia)
                .NotEmpty().WithMessage("O nome fantasia é obrigatório.")
                .Length(3, 100).WithMessage("O nome fantasia deve ter entre 3 e 100 caracteres.");

            RuleFor(e => e.CNPJ)
                .NotEmpty().WithMessage("O CNPJ é obrigatório.")
                .Matches(@"^\d{14}$").WithMessage("O CNPJ deve ter 14 dígitos.");

            RuleFor(e => e.Telefone)
                .NotEmpty().WithMessage("O telefone é obrigatório.")
                .Matches(@"^\d{10,11}$").WithMessage("O telefone deve ter 10 ou 11 dígitos.");

            RuleFor(e => e.Endereco)
                .NotEmpty().WithMessage("O endereço é obrigatório.")
                .Length(5, 200).WithMessage("O endereço deve ter entre 5 e 200 caracteres.");
        }
    }
}