using FluentValidation;
using Web.Domain.DTOs.Fornecedor;

namespace Web.Application.Validators
{
    public class FornecedorUpdateDtoValidator : AbstractValidator<FornecedorUpdateDto>
    {
        public FornecedorUpdateDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("O ID do fornecedor é obrigatório e deve ser maior que zero.");

            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome do fornecedor é obrigatório.")
                .MaximumLength(255).WithMessage("O nome do fornecedor não pode exceder 255 caracteres.");

            RuleFor(x => x.CNPJ)
                .NotEmpty().WithMessage("O CNPJ do fornecedor é obrigatório.")
                .Matches(@"^\d{2}\.\d{3}\.\d{3}\/\d{4}-\d{2}$").WithMessage("O CNPJ deve estar no formato correto (XX.XXX.XXX/XXXX-XX).")
                .MaximumLength(18).WithMessage("O CNPJ não pode exceder 18 caracteres.");

            RuleFor(x => x.Endereco)
                .NotEmpty().WithMessage("O endereço do fornecedor é obrigatório.")
                .MaximumLength(255).WithMessage("O endereço do fornecedor não pode exceder 255 caracteres.");

            RuleFor(x => x.Telefone)
                .NotEmpty().WithMessage("O telefone do fornecedor é obrigatório.")
                .Matches(@"^\(\d{2}\) \d{4,5}-\d{4}$").WithMessage("O telefone deve estar no formato correto ((XX) XXXX-XXXX ou (XX) XXXXX-XXXX).")
                .MaximumLength(15).WithMessage("O telefone não pode exceder 15 caracteres.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("O email do fornecedor é obrigatório.")
                .EmailAddress().WithMessage("O email deve ser um endereço de email válido.")
                .MaximumLength(255).WithMessage("O email do fornecedor não pode exceder 255 caracteres.");

            RuleFor(x => x.EstabelecimentoId)
                .GreaterThan(0).WithMessage("O ID do estabelecimento é obrigatório e deve ser maior que zero.");
        }
    }
}