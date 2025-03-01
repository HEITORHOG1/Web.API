namespace MarketplaceHybrid.Shared.Services.Interfaces
{
    public interface IHorarioFuncionamentoService
    {
        Task<bool> EstaAbertoAsync(int estabelecimentoId, int diaSemana, string horaAtual);
    }
}
