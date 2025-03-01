using Web.Domain.DTOs;

namespace Web.Application.Interfaces
{
    public interface IOpcaoProdutoService
    {
        Task<List<OpcaoProdutoDto>> GetAllByProdutoIdAsync(int estabelecimentoId, int produtoId);

        Task<OpcaoProdutoDto> GetOpcaoByIdAsync(int estabelecimentoId, int produtoId, int opcaoId);

        Task AddOpcaoAsync(int estabelecimentoId, int produtoId, OpcaoProdutoDto opcaoDto);

        Task UpdateOpcaoAsync(int estabelecimentoId, int produtoId, OpcaoProdutoDto opcaoDto);

        Task RemoveOpcaoAsync(int estabelecimentoId, int produtoId, int opcaoId);

        Task ReplicarOpcoesParaProdutosDaCategoriaAsync(int categoriaId, int produtoOrigemId);
    }
}