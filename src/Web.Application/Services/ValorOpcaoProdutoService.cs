using Web.Application.Interfaces;
using Web.Domain.DTOs;
using Web.Domain.Entities;
using Web.Domain.Interfaces;

namespace Web.Application.Services
{
    public class ValorOpcaoProdutoService : IValorOpcaoProdutoService
    {
        private readonly IValorOpcaoProdutoRepository _valorOpcaoProdutoRepository;

        public ValorOpcaoProdutoService(IValorOpcaoProdutoRepository valorOpcaoProdutoRepository)
        {
            _valorOpcaoProdutoRepository = valorOpcaoProdutoRepository;
        }

        public async Task<List<ValorOpcaoProdutoDto>> GetAllByOpcaoAsync(int estabelecimentoId, int produtoId, int opcaoId)
        {
            var valores = await _valorOpcaoProdutoRepository.GetAllByOpcaoIdAsync(estabelecimentoId, produtoId, opcaoId);

            return valores.Select(v => new ValorOpcaoProdutoDto
            {
                Id = v.Id,
                OpcaoProdutoId = v.OpcaoProdutoId,
                Descricao = v.Descricao,
                PrecoAdicional = v.PrecoAdicional
            }).ToList();
        }

        public async Task<ValorOpcaoProdutoDto> GetValorByIdAsync(int estabelecimentoId, int produtoId, int opcaoId, int valorId)
        {
            var valor = await _valorOpcaoProdutoRepository.GetValorByIdAsync(estabelecimentoId, produtoId, opcaoId, valorId);
            if (valor == null)
                throw new Exception("Valor não encontrado.");

            return new ValorOpcaoProdutoDto
            {
                Id = valor.Id,
                OpcaoProdutoId = valor.OpcaoProdutoId,
                Descricao = valor.Descricao,
                PrecoAdicional = valor.PrecoAdicional
            };
        }

        public async Task AddValorAsync(int estabelecimentoId, int produtoId, int opcaoId, ValorOpcaoProdutoDto valorDto)
        {
            var valor = new ValorOpcaoProduto
            {
                Descricao = valorDto.Descricao,
                PrecoAdicional = valorDto.PrecoAdicional
            };

            await _valorOpcaoProdutoRepository.AddValorAsync(estabelecimentoId, produtoId, opcaoId, valor);
        }

        public async Task UpdateValorAsync(int estabelecimentoId, int produtoId, int opcaoId, ValorOpcaoProdutoDto valorDto)
        {
            var valor = new ValorOpcaoProduto
            {
                Id = valorDto.Id,
                OpcaoProdutoId = valorDto.OpcaoProdutoId,
                Descricao = valorDto.Descricao,
                PrecoAdicional = valorDto.PrecoAdicional
            };

            await _valorOpcaoProdutoRepository.UpdateValorAsync(estabelecimentoId, produtoId, opcaoId, valor);
        }

        public async Task RemoveValorAsync(int estabelecimentoId, int produtoId, int opcaoId, int valorId)
        {
            await _valorOpcaoProdutoRepository.RemoveValorAsync(estabelecimentoId, produtoId, opcaoId, valorId);
        }
    }
}