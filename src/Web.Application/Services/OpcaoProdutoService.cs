using Web.Application.Interfaces;
using Web.Domain.DTOs;
using Web.Domain.Entities;
using Web.Domain.Interfaces;

namespace Web.Application.Services
{
    public class OpcaoProdutoService : IOpcaoProdutoService
    {
        private readonly IOpcaoProdutoRepository _opcaoProdutoRepository;

        public OpcaoProdutoService(IOpcaoProdutoRepository opcaoProdutoRepository)
        {
            _opcaoProdutoRepository = opcaoProdutoRepository;
        }

        public async Task<List<OpcaoProdutoDto>> GetAllByProdutoIdAsync(int estabelecimentoId, int produtoId)
        {
            var opcoes = await _opcaoProdutoRepository.GetAllByProdutoIdAsync(estabelecimentoId, produtoId);

            return opcoes.Select(o => new OpcaoProdutoDto
            {
                Id = o.Id,
                ProdutoId = o.ProdutoId,
                Nome = o.Nome,
                Obrigatorio = o.Obrigatorio,
                Valores = o.Valores.Select(v => new ValorOpcaoProdutoDto
                {
                    Id = v.Id,
                    OpcaoProdutoId = v.OpcaoProdutoId,
                    Descricao = v.Descricao,
                    PrecoAdicional = v.PrecoAdicional
                }).ToList()
            }).ToList();
        }

        public async Task<OpcaoProdutoDto> GetOpcaoByIdAsync(int estabelecimentoId, int produtoId, int opcaoId)
        {
            var opcao = await _opcaoProdutoRepository.GetOpcaoByIdAsync(estabelecimentoId, produtoId, opcaoId);
            if (opcao == null)
                throw new Exception("Opção não encontrada.");

            return new OpcaoProdutoDto
            {
                Id = opcao.Id,
                ProdutoId = opcao.ProdutoId,
                Nome = opcao.Nome,
                Obrigatorio = opcao.Obrigatorio,
                Valores = opcao.Valores.Select(v => new ValorOpcaoProdutoDto
                {
                    Id = v.Id,
                    OpcaoProdutoId = v.OpcaoProdutoId,
                    Descricao = v.Descricao,
                    PrecoAdicional = v.PrecoAdicional
                }).ToList()
            };
        }

        public async Task AddOpcaoAsync(int estabelecimentoId, int produtoId, OpcaoProdutoDto opcaoDto)
        {
            var opcao = new OpcaoProduto
            {
                Nome = opcaoDto.Nome,
                Obrigatorio = opcaoDto.Obrigatorio
            };

            // Caso queira adicionar Valores também, pode-se mapear aqui
            opcao.Valores = opcaoDto.Valores.Select(v => new ValorOpcaoProduto
            {
                Descricao = v.Descricao,
                PrecoAdicional = v.PrecoAdicional
            }).ToList();

            await _opcaoProdutoRepository.AddOpcaoAsync(estabelecimentoId, produtoId, opcao);
        }

        public async Task UpdateOpcaoAsync(int estabelecimentoId, int produtoId, OpcaoProdutoDto opcaoDto)
        {
            var opcao = new OpcaoProduto
            {
                Id = opcaoDto.Id,
                ProdutoId = opcaoDto.ProdutoId,
                Nome = opcaoDto.Nome,
                Obrigatorio = opcaoDto.Obrigatorio,
            };

            // Aqui, caso a lógica de atualização dos valores seja necessária, terá que haver uma estratégia adequada, pois pode envolver adicionar, remover ou atualizar valores.
            // Por simplicidade, este exemplo não atualiza valores diretamente, mas poderia ser implementado conforme a regra de negócio.

            await _opcaoProdutoRepository.UpdateOpcaoAsync(estabelecimentoId, produtoId, opcao);
        }

        public async Task RemoveOpcaoAsync(int estabelecimentoId, int produtoId, int opcaoId)
        {
            await _opcaoProdutoRepository.RemoveOpcaoAsync(estabelecimentoId, produtoId, opcaoId);
        }

        public async Task ReplicarOpcoesParaProdutosDaCategoriaAsync(int categoriaId, int produtoOrigemId)
        {
            await _opcaoProdutoRepository.ReplicarOpcoesParaProdutosDaCategoriaAsync(categoriaId, produtoOrigemId);
        }
    }
}