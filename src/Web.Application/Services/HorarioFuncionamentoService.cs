using Web.Application.Interfaces;
using Web.Domain.Entities;
using Web.Domain.Interfaces;

namespace Web.Application.Services
{
    public class HorarioFuncionamentoService : IHorarioFuncionamentoService
    {
        private readonly IHorarioFuncionamentoRepository _horarioFuncionamentoRepository;

        public HorarioFuncionamentoService(IHorarioFuncionamentoRepository horarioFuncionamentoRepository)
        {
            _horarioFuncionamentoRepository = horarioFuncionamentoRepository;
        }

        public async Task<IEnumerable<HorarioFuncionamento>> GetByEstabelecimentoIdAsync(int estabelecimentoId)
        {
            return await _horarioFuncionamentoRepository.GetByEstabelecimentoIdAsync(estabelecimentoId);
        }

        public async Task<bool> EstaAbertoAsync(int estabelecimentoId, DayOfWeek diaSemana, TimeSpan horaAtual)
        {
            return await _horarioFuncionamentoRepository.EstaAbertoAsync(estabelecimentoId, diaSemana, horaAtual);
        }

        public async Task AddAsync(HorarioFuncionamento horario)
        {
            await _horarioFuncionamentoRepository.AddAsync(horario);
        }

        public async Task UpdateAsync(HorarioFuncionamento horario)
        {
            await _horarioFuncionamentoRepository.UpdateAsync(horario);
        }

        public async Task DeleteAsync(int id)
        {
            await _horarioFuncionamentoRepository.DeleteAsync(id);
        }
    }
}