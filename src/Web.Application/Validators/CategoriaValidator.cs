using FluentValidation;
using Web.Domain.Entities;

namespace Web.Application.Validators
{
    public class CategoriaValidator : AbstractValidator<Categoria>
    {
        public CategoriaValidator()
        {
            RuleFor(c => c.Nome)
                .NotEmpty().WithMessage("O nome da categoria é obrigatório.")
                .Length(3, 50).WithMessage("O nome da categoria deve ter entre 3 e 50 caracteres.");

            RuleFor(c => c.Descricao)
                .NotEmpty().WithMessage("A descrição da categoria é obrigatória.")
                .MaximumLength(200).WithMessage("A descrição da categoria deve ter no máximo 200 caracteres.");

            RuleFor(c => c.EstabelecimentoId)
                .GreaterThan(0).WithMessage("O ID do estabelecimento deve ser válido.");
        }
    }
}