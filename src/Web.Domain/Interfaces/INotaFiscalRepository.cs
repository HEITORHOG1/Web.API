using Web.Domain.Entities;

namespace Web.Domain.Interfaces
{
    public interface INotaFiscalRepository : IGenericRepository<NotaFiscal>
    {
        Task<IEnumerable<NotaFiscal>> GetAllByEstabelecimentoIdAsync(int estabelecimentoId);
    }
}