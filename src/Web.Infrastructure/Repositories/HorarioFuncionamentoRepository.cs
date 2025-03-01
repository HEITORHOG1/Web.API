using Microsoft.EntityFrameworkCore;
using Web.Domain.Entities;
using Web.Domain.Interfaces;
using Web.Infrastructure.Data.Context;

namespace Web.Infrastructure.Repositories
{
    public class HorarioFuncionamentoRepository : GenericRepository<HorarioFuncionamento>, IHorarioFuncionamentoRepository
    {
        private readonly AppDbContext _context;

        public HorarioFuncionamentoRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HorarioFuncionamento>> GetByEstabelecimentoIdAsync(int estabelecimentoId)
        {
            return await _dbSet
                .Where(h => h.EstabelecimentoId == estabelecimentoId)
                .OrderBy(h => h.DiaSemana)
                .ThenBy(h => h.HoraAbertura)
                .ToListAsync();
        }

        public async Task<bool> EstaAbertoAsync(int estabelecimentoId, DayOfWeek diaSemana, TimeSpan horaAtual)
        {
            // Adiciona lógica para considerar horários que cruzam meia-noite
            return await _dbSet
                .AnyAsync(h =>
                    h.EstabelecimentoId == estabelecimentoId &&
                    h.DiaSemana == diaSemana &&
                    (
                        // Aberto em horário normal (mesmo dia)
                        (h.HoraAbertura <= horaAtual && h.HoraFechamento > horaAtual) ||
                        // Aberto cruzando meia-noite
                        (h.HoraAbertura > h.HoraFechamento &&
                         (horaAtual >= h.HoraAbertura || horaAtual < h.HoraFechamento))
                    ));
        }

    }
}