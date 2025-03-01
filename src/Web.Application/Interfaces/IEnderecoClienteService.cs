using Web.Domain.DTOs.Endereco;
using Web.Domain.Entities;

namespace Web.Application.Interfaces
{
    public interface IEnderecoClienteService
    {
        Task<IEnumerable<EnderecoCliente>> GetAllByUsuarioIdAsync(string usuarioId);

        Task<EnderecoCliente> AddEnderecoAsync(string userId, EnderecoClienteDto enderecoDto);

        Task<EnderecoCliente> GetPrincipalByUsuarioIdAsync(string usuarioId);

        Task<EnderecoCliente> GetEnderecoByIdAsync(int id);

        Task UpdateEnderecoAsync(int id, string userId, EnderecoClienteDto enderecoDto);

        Task DeleteEnderecoAsync(int id);
    }
}