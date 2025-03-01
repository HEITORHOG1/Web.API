namespace Web.Domain.DTOs
{
    public class CreateHorarioFuncionamento
    {
        public int EstabelecimentoId { get; set; } // FK para Estabelecimento
        public DayOfWeek DiaSemana { get; set; } // Dia da semana
        public TimeSpan HoraAbertura { get; set; } // Hora de abertura
        public TimeSpan HoraFechamento { get; set; } // Hora de fechamento
    }
}