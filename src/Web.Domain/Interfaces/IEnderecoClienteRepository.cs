using Web.Domain.Entities;

namespace Web.Domain.Interfaces
{
    public interface IEnderecoClienteRepository : IGenericRepository<EnderecoCliente>
    {
        Task<IEnumerable<EnderecoCliente>> GetAllByUsuarioIdAsync(string usuarioId);

        Task<EnderecoCliente> GetPrincipalByUsuarioIdAsync(string usuarioId);
    }
}