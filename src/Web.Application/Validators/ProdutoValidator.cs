using FluentValidation;
using Web.Domain.Entities;

namespace Web.Application.Validators
{
    public class ProdutoValidator : AbstractValidator<Produto>
    {
        public ProdutoValidator()
        {
            RuleFor(p => p.Nome)
                .NotEmpty().WithMessage("O nome do produto é obrigatório.")
                .Length(3, 100).WithMessage("O nome do produto deve ter entre 3 e 100 caracteres.");

            RuleFor(p => p.Descricao)
                .MaximumLength(500).WithMessage("A descrição do produto deve ter no máximo 500 caracteres.");

            RuleFor(p => p.Preco)
                .GreaterThan(0).WithMessage("O preço do produto deve ser maior que zero.");

            RuleFor(p => p.QuantidadeEmEstoque)
                .GreaterThanOrEqualTo(0).WithMessage("A quantidade em estoque não pode ser negativa.");

            RuleFor(p => p.CategoriaId)
                .GreaterThan(0).WithMessage("O ID da categoria deve ser válido.");

            RuleFor(p => p.EstabelecimentoId)
                .GreaterThan(0).WithMessage("O ID do estabelecimento deve ser válido.");
        }
    }
}