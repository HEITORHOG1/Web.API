using FluentValidation;
using Web.Domain.DTOs.NotaFiscal;

namespace Web.API.Validators
{
    public class NotaFiscalDtoValidator : AbstractValidator<NotaFiscalDto>
    {
        public NotaFiscalDtoValidator()
        {
            RuleFor(n => n.Numero)
                .NotEmpty().WithMessage("O número da nota fiscal é obrigatório.")
                .MaximumLength(50).WithMessage("O número da nota fiscal deve ter no máximo 50 caracteres.");

            RuleFor(n => n.DataEmissao)
                .NotEmpty().WithMessage("A data de emissão é obrigatória.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("A data de emissão não pode ser futura.");

            RuleFor(n => n.EstabelecimentoId)
                .GreaterThan(0).WithMessage("O ID do estabelecimento deve ser maior que zero.");

            RuleFor(n => n.FornecedorId)
                .GreaterThan(0).WithMessage("O ID do fornecedor deve ser maior que zero.");

            RuleFor(n => n.Produtos)
                .NotEmpty().WithMessage("A nota fiscal deve conter pelo menos um produto.");
        }
    }
}