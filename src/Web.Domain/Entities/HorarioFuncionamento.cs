using System.Text.Json.Serialization;

namespace Web.Domain.Entities
{
    public class HorarioFuncionamento
    {
        public int Id { get; set; } // Chave primária
        public int EstabelecimentoId { get; set; } // FK para Estabelecimento
        public DayOfWeek DiaSemana { get; set; } // Dia da semana
        public TimeSpan HoraAbertura { get; set; } // Hora de abertura
        public TimeSpan HoraFechamento { get; set; } // Hora de fechamento

        [JsonIgnore]
        public virtual Estabelecimento Estabelecimento { get; set; } // Navegação
    }
}