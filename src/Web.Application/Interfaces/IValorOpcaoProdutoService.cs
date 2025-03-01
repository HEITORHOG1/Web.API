using Web.Domain.DTOs;

namespace Web.Application.Interfaces
{
    public interface IValorOpcaoProdutoService
    {
        Task<List<ValorOpcaoProdutoDto>> GetAllByOpcaoAsync(int estabelecimentoId, int produtoId, int opcaoId);

        Task<ValorOpcaoProdutoDto> GetValorByIdAsync(int estabelecimentoId, int produtoId, int opcaoId, int valorId);

        Task AddValorAsync(int estabelecimentoId, int produtoId, int opcaoId, ValorOpcaoProdutoDto valorDto);

        Task UpdateValorAsync(int estabelecimentoId, int produtoId, int opcaoId, ValorOpcaoProdutoDto valorDto);

        Task RemoveValorAsync(int estabelecimentoId, int produtoId, int opcaoId, int valorId);
    }
}