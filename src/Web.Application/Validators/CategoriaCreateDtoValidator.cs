using FluentValidation;
using Web.Domain.DTOs.Categorias;

namespace Web.API.Validators
{
    public class CategoriaCreateDtoValidator : AbstractValidator<CategoriaCreateDto>
    {
        public CategoriaCreateDtoValidator()
        {
            RuleFor(c => c.Nome)
                .NotEmpty().WithMessage("O nome da categoria é obrigatório.")
                .MaximumLength(100).WithMessage("O nome da categoria deve ter no máximo 100 caracteres.");

            RuleFor(c => c.Descricao)
                .MaximumLength(500).WithMessage("A descrição deve ter no máximo 500 caracteres.");
        }
    }
}