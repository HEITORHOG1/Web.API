using FluentValidation;
using Web.Domain.DTOs.Produtos;

namespace Web.Application.Validators
{
    public class ProdutoCreateDtoValidator : AbstractValidator<ProdutoCreateDto>
    {
        public ProdutoCreateDtoValidator()
        {
            RuleFor(p => p.Nome)
                .NotEmpty().WithMessage("O nome do produto é obrigatório.")
                .MaximumLength(100).WithMessage("O nome do produto deve ter no máximo 100 caracteres.");

            RuleFor(p => p.Descricao)
                .MaximumLength(500).WithMessage("A descrição deve ter no máximo 500 caracteres.");

            RuleFor(p => p.Preco)
                .GreaterThan(0.00m).WithMessage("O preço do produto é obrigatório e deve ser maior que zero.");

            RuleFor(p => p.CategoriaId)
                .GreaterThan(0).WithMessage("A categoria é obrigatória.");
        }
    }
}