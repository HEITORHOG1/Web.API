using MarketplaceHybrid.Shared.Models;

namespace MarketplaceHybrid.Shared.Services.Interfaces
{
    public interface IEnderecoClienteService
    {
        Task<EnderecoClienteResponse?> AddEnderecoAsync(EnderecoClienteDto enderecoDto, string token);

        Task<IEnumerable<EnderecoClienteDto>?> GetAllByUsuarioIdAsync(string token);

        Task<EnderecoClienteDto?> GetPrincipalByUsuarioIdAsync(string userId);

        Task<EnderecoClienteResponse?> UpdateEnderecoAsync(EnderecoClienteDto enderecoDto, string token);
    }
}