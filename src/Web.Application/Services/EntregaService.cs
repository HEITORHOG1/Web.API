using FluentValidation;
using Web.Application.Interfaces;
using Web.Application.Validators;
using Web.Domain.Entities;
using Web.Domain.Interfaces;

namespace Web.Application.Services
{
    public class EntregaService : IEntregaService
    {
        private readonly IEntregaRepository _entregaRepository;
        private readonly EntregaValidator _entregaValidator;

        public EntregaService(IEntregaRepository entregaRepository, EntregaValidator entregaValidator)
        {
            _entregaRepository = entregaRepository;
            _entregaValidator = entregaValidator;
        }

        public async Task AddAsync(Entrega entrega)
        {
            var validationResult = _entregaValidator.Validate(entrega);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            await _entregaRepository.AddAsync(entrega);
        }

        public async Task<Entrega> GetByPedidoIdAsync(int pedidoId)
        {
            return await _entregaRepository.GetByPedidoIdAsync(pedidoId);
        }

        public async Task AtualizarStatusEntregaAsync(int entregaId, StatusEntrega novoStatus)
        {
            var entrega = await _entregaRepository.GetByIdAsync(entregaId);
            if (entrega == null)
            {
                throw new Exception("Entrega não encontrada");
            }

            entrega.Status = novoStatus;

            if (novoStatus == StatusEntrega.Entregue)
            {
                entrega.DataHoraEntrega = DateTime.UtcNow;
            }

            await _entregaRepository.UpdateAsync(entrega);
        }

        // Implementar outros métodos conforme necessário
    }
}