using FluentValidation;
using Web.Domain.Entities;

namespace Web.Application.Validators
{
    public class ImagemProdutoValidator : AbstractValidator<ImagemProduto>
    {
        public ImagemProdutoValidator()
        {
            RuleFor(x => x.ProdutoId)
                .NotEmpty()
                .WithMessage("O ID do produto é obrigatório");

            RuleFor(x => x.EstabelecimentoId)
                .NotEmpty()
                .WithMessage("O ID do estabelecimento é obrigatório");

            RuleFor(x => x.Url)
                .NotEmpty()
                .WithMessage("A URL da imagem é obrigatória");

            RuleFor(x => x.DataCadastro)
                .NotEmpty()
                .WithMessage("A data de cadastro é obrigatória");
        }
    }
}