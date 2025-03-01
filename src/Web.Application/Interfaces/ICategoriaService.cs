using Web.Domain.DTOs.Categorias;
using Web.Domain.Entities;

namespace Web.Application.Interfaces
{
    public interface ICategoriaService
    {
        Task<CategoriaDto> GetCategoriaByIdAsync(int estabelecimentoId, int id);
        Task<IEnumerable<CategoriaDto>> GetAllByEstabelecimentoIdAsync(int estabelecimentoId);
        Task<IEnumerable<CategoriaDto>> GetAllCategoriasAsync();
        Task<CategoriaDto> CreateCategoriaAsync(int estabelecimentoId, string userId, CategoriaCreateDto categoriaDto);
        Task UpdateCategoriaAsync(int estabelecimentoId, int id, string userId, CategoriaUpdateDto categoriaDto);
        Task DeleteCategoriaAsync(int estabelecimentoId, int id, string userId);
        Task<bool> ExistsAsync(int estabelecimentoId, string nome);
        Task<IEnumerable<CategoriaDto>> GetCategoriasByEstabelecimentoIdAsync(int estabelecimentoId);
    }
}