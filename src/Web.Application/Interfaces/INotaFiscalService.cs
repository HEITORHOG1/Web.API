using Web.Domain.Entities;

namespace Web.Application.Interfaces
{
    public interface INotaFiscalService
    {
        Task<NotaFiscal> GetByIdAsync(int id);

        Task AddNotaFiscalAsync(NotaFiscal notaFiscal);

        Task UpdateNotaFiscalAsync(NotaFiscal notaFiscal);

        Task DeleteNotaFiscalAsync(int id);

        Task<IEnumerable<NotaFiscal>> GetAllByEstabelecimentoIdAsync(int estabelecimentoId);
    }
}