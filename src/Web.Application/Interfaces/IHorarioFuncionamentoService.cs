using Web.Domain.Entities;

namespace Web.Application.Interfaces
{
    public interface IHorarioFuncionamentoService
    {
        Task<IEnumerable<HorarioFuncionamento>> GetByEstabelecimentoIdAsync(int estabelecimentoId);

        Task<bool> EstaAbertoAsync(int estabelecimentoId, DayOfWeek diaSemana, TimeSpan horaAtual);

        Task AddAsync(HorarioFuncionamento horario);

        Task UpdateAsync(HorarioFuncionamento horario);

        Task DeleteAsync(int id);
    }
}