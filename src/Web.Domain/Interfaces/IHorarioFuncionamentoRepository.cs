using Web.Domain.Entities;

namespace Web.Domain.Interfaces
{
    public interface IHorarioFuncionamentoRepository : IGenericRepository<HorarioFuncionamento>
    {
        Task<IEnumerable<HorarioFuncionamento>> GetByEstabelecimentoIdAsync(int estabelecimentoId);

        Task<bool> EstaAbertoAsync(int estabelecimentoId, DayOfWeek diaSemana, TimeSpan horaAtual);
    }
}