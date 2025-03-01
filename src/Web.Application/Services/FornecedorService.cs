using Web.Application.Interfaces;
using Web.Domain.Entities;
using Web.Domain.Interfaces;

namespace Web.Application.Services
{
    public class FornecedorService : IFornecedorService
    {
        private readonly IFornecedorRepository _fornecedorRepository;

        public FornecedorService(IFornecedorRepository fornecedorRepository)
        {
            _fornecedorRepository = fornecedorRepository;
        }

        public async Task AddFornecedorAsync(Fornecedor fornecedor)
        {
            try
            {
                await _fornecedorRepository.AddAsync(fornecedor);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task DeleteFornecedorAsync(int id)
        {
            var fornecedor = await _fornecedorRepository.GetByIdAsync(id);
            if (fornecedor != null)
            {
                await _fornecedorRepository.DeleteAsync(id);
            }
        }

        public async Task<IEnumerable<Fornecedor>> GetAllAsync()
        {
            return await _fornecedorRepository.GetAllAsync();
        }

        public async Task<Fornecedor> GetByIdAsync(int id)
        {
            return await _fornecedorRepository.GetByIdAsync(id);
        }

        public async Task UpdateFornecedorAsync(Fornecedor fornecedor)
        {
            await _fornecedorRepository.UpdateAsync(fornecedor);
        }

        public async Task<IEnumerable<Fornecedor>> GetAllByEstabelecimentoIdAsync(int estabelecimentoId)
        {
            return await _fornecedorRepository.GetAllByEstabelecimentoIdAsync(estabelecimentoId);
        }

        public async Task<bool> ExistsAsync(int estabelecimentoId, string nome)
        {
            return await _fornecedorRepository.ExistsAsync(estabelecimentoId, nome);
        }

        public async Task<bool> ExistsAsyncCNPJ(int estabelecimentoId, string cnpj)
        {
            return await _fornecedorRepository.ExistsAsyncCNPJ(estabelecimentoId, cnpj);
        }
    }
}