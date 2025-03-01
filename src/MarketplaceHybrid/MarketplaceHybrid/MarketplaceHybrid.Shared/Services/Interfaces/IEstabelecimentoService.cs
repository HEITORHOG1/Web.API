using MarketplaceHybrid.Shared.Models;

namespace MarketplaceHybrid.Shared.Services.Interfaces
{
    public interface IEstabelecimentoService
    {
        Task<EstabelecimentoResponse?> GetEstabelecimentoByIdAsync(int id);

        Task<EnderecoDto> BuscarEnderecoPorCepAsync(string cep);
    }
}