using FluentValidation;
using Web.Domain.Entities;

namespace Web.Application.Validators
{
    public class EnderecoClienteValidator : AbstractValidator<EnderecoCliente>
    {
        public EnderecoClienteValidator()
        {
            RuleFor(endereco => endereco.Logradouro)
                 .NotEmpty()
                 .WithMessage("Logradouro é obrigatório.")
                 .MaximumLength(255)
                 .WithMessage("O logradouro não pode exceder 255 caracteres.");

            RuleFor(endereco => endereco.Numero)
                .NotEmpty()
                .WithMessage("Número é obrigatório.")
                .MaximumLength(10)
                 .WithMessage("O número não pode exceder 10 caracteres.");

            RuleFor(endereco => endereco.Bairro)
                .NotEmpty()
                 .WithMessage("O bairro é obrigatório")
                .MaximumLength(100)
                 .WithMessage("O bairro não pode exceder 100 caracteres.");

            RuleFor(endereco => endereco.Cidade)
                .NotEmpty()
                .WithMessage("A cidade é obrigatória")
                .MaximumLength(100)
                 .WithMessage("A cidade não pode exceder 100 caracteres.");

            RuleFor(endereco => endereco.Estado)
                .NotEmpty()
               .WithMessage("O estado é obrigatório")
               .MaximumLength(2)
               .WithMessage("O estado não pode exceder 2 caracteres.");

            RuleFor(endereco => endereco.CEP)
              .NotEmpty()
                .WithMessage("CEP é obrigatório")
               .Matches(@"^\d{5}-\d{3}$")
              .WithMessage("Formato de CEP inválido.");
        }
    }
}