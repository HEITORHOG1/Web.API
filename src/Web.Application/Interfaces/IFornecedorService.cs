using Web.Domain.Entities;

namespace Web.Application.Interfaces
{
    public interface IFornecedorService
    {
        Task AddFornecedorAsync(Fornecedor fornecedor);

        Task DeleteFornecedorAsync(int id);

        Task<IEnumerable<Fornecedor>> GetAllAsync();

        Task<IEnumerable<Fornecedor>> GetAllByEstabelecimentoIdAsync(int estabelecimentoId); // Novo método

        Task<Fornecedor> GetByIdAsync(int id);

        Task UpdateFornecedorAsync(Fornecedor fornecedor);

        Task<bool> ExistsAsync(int estabelecimentoId, string nome);

        Task<bool> ExistsAsyncCNPJ(int estabelecimentoId, string cnpj);
    }
}