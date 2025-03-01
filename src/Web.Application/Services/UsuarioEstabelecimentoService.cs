using Web.Application.Interfaces;
using Web.Domain.Entities;
using Web.Domain.Interfaces;

namespace Web.Application.Services
{
    public class UsuarioEstabelecimentoService : IUsuarioEstabelecimentoService
    {
        private readonly IUsuarioEstabelecimentoRepository _usuarioEstabelecimentoRepository;

        public UsuarioEstabelecimentoService(IUsuarioEstabelecimentoRepository usuarioEstabelecimentoRepository)
        {
            _usuarioEstabelecimentoRepository = usuarioEstabelecimentoRepository;
        }

        public async Task<IEnumerable<UsuarioEstabelecimento>> GetAllAsync()
        {
            return await _usuarioEstabelecimentoRepository.GetAllAsync();
        }

        public async Task<UsuarioEstabelecimento> GetByIdAsync(int id)
        {
            return await _usuarioEstabelecimentoRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(UsuarioEstabelecimento usuarioEstabelecimento)
        {
            await _usuarioEstabelecimentoRepository.AddAsync(usuarioEstabelecimento);
        }

        public async Task UpdateAsync(UsuarioEstabelecimento usuarioEstabelecimento)
        {
            await _usuarioEstabelecimentoRepository.UpdateAsync(usuarioEstabelecimento);
        }

        public async Task DeleteAsync(int id)
        {
            await _usuarioEstabelecimentoRepository.DeleteAsync(id);
        }

        public async Task<bool> IsProprietarioAsync(string userId, int estabelecimentoId)
        {
            return await _usuarioEstabelecimentoRepository.IsProprietarioAsync(userId, estabelecimentoId);
        }

        public async Task<IEnumerable<UsuarioEstabelecimento>> GetByEstabelecimentoIdAsync(int estabelecimentoId)
        {
            return await _usuarioEstabelecimentoRepository.GetByEstabelecimentoIdAsync(estabelecimentoId);
        }

        public async Task<UsuarioEstabelecimento> GetVinculoAsync(string userId, int estabelecimentoId)
        {
            return await _usuarioEstabelecimentoRepository.GetVinculoAsync(userId, estabelecimentoId);
        }
    }
}