using Web.Domain.Entities;

namespace Web.Domain.Interfaces
{
    public interface IFornecedorRepository : IGenericRepository<Fornecedor>
    {
        Task<IEnumerable<Fornecedor>> GetAllByEstabelecimentoIdAsync(int estabelecimentoId); // Novo método

        Task<bool> ExistsAsync(int estabelecimentoId, string nome);

        Task<bool> ExistsAsyncCNPJ(int estabelecimentoId, string cnpj);
    }
}