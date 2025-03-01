using FluentValidation;
using Web.Application.Interfaces;
using Web.Application.Validators;
using Web.Domain.Entities;
using Web.Domain.Interfaces;

namespace Web.Application.Services
{
    public class EntregadorService : IEntregadorService
    {
        private readonly IEntregadorRepository _entregadorRepository;
        private readonly EntregadorValidator _validationRules;

        public EntregadorService(IEntregadorRepository entregadorRepository, EntregadorValidator validationRules)
        {
            _entregadorRepository = entregadorRepository;
            _validationRules = validationRules;
        }

        public async Task AddAsync(Entregador entregador)
        {
            var validationResult = _validationRules.Validate(entregador);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
            await _entregadorRepository.AddAsync(entregador);
        }

        public async Task<Entregador> GetByIdAsync(int entregadorId)
        {
            return await _entregadorRepository.GetByIdAsync(entregadorId);
        }

        public async Task<IEnumerable<Entregador>> GetEntregadoresByEstabelecimentoIdAsync(int estabelecimentoId)
        {
            return await _entregadorRepository.GetEntregadoresByEstabelecimentoIdAsync(estabelecimentoId);
        }

        // Implementar outros métodos conforme necessário
    }
}