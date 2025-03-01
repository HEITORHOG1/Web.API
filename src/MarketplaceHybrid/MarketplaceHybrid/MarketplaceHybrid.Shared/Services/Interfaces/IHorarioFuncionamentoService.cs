using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketplaceHybrid.Shared.Services.Interfaces
{
    public interface IHorarioFuncionamentoService
    {
        Task<bool> EstaAbertoAsync(int estabelecimentoId, int diaSemana, string horaAtual);
    }
}
