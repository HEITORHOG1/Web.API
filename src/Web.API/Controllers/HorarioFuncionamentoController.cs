using Microsoft.AspNetCore.Mvc;
using Web.Application.Interfaces;
using Web.Domain.DTOs;
using Web.Domain.Entities;

namespace Web.API.Controllers
{
    /// <summary>
    /// Controller para gerenciar os horários de funcionamento dos estabelecimentos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HorarioFuncionamentoController : ControllerBase
    {
        private readonly IHorarioFuncionamentoService _horarioFuncionamentoService;
        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="horarioFuncionamentoService"></param>
        public HorarioFuncionamentoController(IHorarioFuncionamentoService horarioFuncionamentoService)
        {
            _horarioFuncionamentoService = horarioFuncionamentoService;
        }
        /// <summary>
        /// Retorna todos os horários de funcionamento de um estabelecimento.
        /// </summary>
        /// <param name="estabelecimentoId"></param>
        /// <returns></returns>
        [HttpGet("estabelecimento/{estabelecimentoId}")]
        public async Task<IActionResult> GetByEstabelecimentoId(int estabelecimentoId)
        {
            var horarios = await _horarioFuncionamentoService.GetByEstabelecimentoIdAsync(estabelecimentoId);
            return Ok(horarios);
        }
        /// <summary>
        /// Verifica se um estabelecimento está aberto em um determinado dia e horário.
        /// </summary>
        /// <param name="estabelecimentoId"></param>
        /// <param name="diaSemana"></param>
        /// <param name="horaAtual"></param>
        /// <returns></returns>
        [HttpGet("esta-aberto")]
        public async Task<IActionResult> EstaAberto(int estabelecimentoId, DayOfWeek diaSemana, TimeSpan horaAtual)
        {
            var aberto = await _horarioFuncionamentoService.EstaAbertoAsync(estabelecimentoId, diaSemana, horaAtual);
            return Ok(new { Aberto = aberto });
        }
        /// <summary>
        /// Adiciona um novo horário de funcionamento.
        /// </summary>
        /// <param name="horario"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Add(CreateHorarioFuncionamento horario)
        {
            // Verifica se já existe um horário para o mesmo dia e estabelecimento
            var horarioExistente = await _horarioFuncionamentoService.GetByEstabelecimentoIdAsync(horario.EstabelecimentoId);
            var horarioParaAtualizar = horarioExistente.FirstOrDefault(h => h.DiaSemana == horario.DiaSemana);

            if (horarioParaAtualizar != null)
            {
                // Atualiza o horário existente
                horarioParaAtualizar.HoraAbertura = horario.HoraAbertura;
                horarioParaAtualizar.HoraFechamento = horario.HoraFechamento;

                await _horarioFuncionamentoService.UpdateAsync(horarioParaAtualizar);

                return Ok(new { Message = "Horário atualizado com sucesso." });
            }
            else
            {
                // Adiciona um novo horário
                HorarioFuncionamento horarioFuncionamento = new HorarioFuncionamento
                {
                    EstabelecimentoId = horario.EstabelecimentoId,
                    DiaSemana = horario.DiaSemana,
                    HoraAbertura = horario.HoraAbertura,
                    HoraFechamento = horario.HoraFechamento
                };

                await _horarioFuncionamentoService.AddAsync(horarioFuncionamento);
                return CreatedAtAction(nameof(GetByEstabelecimentoId), new { estabelecimentoId = horario.EstabelecimentoId }, horario);
            }
        }
        /// <summary>
        /// Atualiza um horário de funcionamento existente.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _horarioFuncionamentoService.DeleteAsync(id);
            return NoContent();
        }
    }
}