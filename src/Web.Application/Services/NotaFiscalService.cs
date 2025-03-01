using FluentValidation;
using FluentValidation.Results;
using Web.Application.Interfaces;
using Web.Application.Validators;
using Web.Domain.Entities;
using Web.Domain.Interfaces;

namespace Web.Application.Services
{
    public class NotaFiscalService : INotaFiscalService
    {
        private readonly INotaFiscalRepository _notaFiscalRepository;
        private readonly INotaFiscalProdutoRepository _notaFiscalProdutoRepository;
        private readonly IProdutoService _produtoService;
        private readonly IMovimentacaoEstoqueService _movimentacaoEstoqueService;
        private readonly NotaFiscalValidator _validator;

        public NotaFiscalService(
            INotaFiscalRepository notaFiscalRepository,
            INotaFiscalProdutoRepository notaFiscalProdutoRepository,
            IProdutoService produtoService,
            IMovimentacaoEstoqueService movimentacaoEstoqueService,
            NotaFiscalValidator validator)
        {
            _notaFiscalRepository = notaFiscalRepository;
            _notaFiscalProdutoRepository = notaFiscalProdutoRepository;
            _produtoService = produtoService;
            _movimentacaoEstoqueService = movimentacaoEstoqueService;
            _validator = validator;
        }

        public async Task<NotaFiscal> GetByIdAsync(int id)
        {
            var notaFiscal = await _notaFiscalRepository.GetByIdAsync(id);
            return notaFiscal;
        }

        public async Task AddNotaFiscalAsync(NotaFiscal notaFiscalDto)
        {
            ValidationResult validationResult = _validator.Validate(notaFiscalDto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var notaFiscal = notaFiscalDto;
            notaFiscal.Produtos = notaFiscalDto.Produtos.Select(p => new NotaFiscalProduto
            {
                ProdutoId = p.ProdutoId,
                Quantidade = p.Quantidade,
                PrecoUnitario = p.PrecoUnitario
            }).ToList();

            await _notaFiscalRepository.AddAsync(notaFiscal);

            foreach (var item in notaFiscal.Produtos)
            {
                await _movimentacaoEstoqueService.RegistrarMovimentacaoAsync(new MovimentacaoEstoque
                {
                    ProdutoId = item.ProdutoId,
                    EstabelecimentoId = notaFiscal.EstabelecimentoId,
                    Quantidade = item.Quantidade,
                    Tipo = TipoMovimentacao.Entrada,
                    Observacao = $"Entrada por Nota Fiscal {notaFiscal.Numero}"
                });
            }
        }

        public async Task UpdateNotaFiscalAsync(NotaFiscal notaFiscalDto)
        {
            ValidationResult validationResult = _validator.Validate(notaFiscalDto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var existingNotaFiscal = await _notaFiscalRepository.GetByIdAsync(notaFiscalDto.Id);
            if (existingNotaFiscal == null)
            {
                throw new KeyNotFoundException("Nota fiscal não encontrada");
            }

            existingNotaFiscal.Numero = notaFiscalDto.Numero;
            existingNotaFiscal.DataEmissao = notaFiscalDto.DataEmissao;
            existingNotaFiscal.Produtos.Clear();

            foreach (var item in notaFiscalDto.Produtos)
            {
                var notaFiscalProduto = new NotaFiscalProduto
                {
                    ProdutoId = item.ProdutoId,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = item.PrecoUnitario
                };
                existingNotaFiscal.Produtos.Add(notaFiscalProduto);
            }

            await _notaFiscalRepository.UpdateAsync(existingNotaFiscal);
        }

        public async Task DeleteNotaFiscalAsync(int id)
        {
            var existingNotaFiscal = await _notaFiscalRepository.GetByIdAsync(id);
            if (existingNotaFiscal != null)
            {
                await _notaFiscalRepository.DeleteAsync(existingNotaFiscal.Id);
            }
        }

        public async Task<IEnumerable<NotaFiscal>> GetAllByEstabelecimentoIdAsync(int estabelecimentoId)
        {
            return await _notaFiscalRepository.GetAllByEstabelecimentoIdAsync(estabelecimentoId);
        }
    }
}