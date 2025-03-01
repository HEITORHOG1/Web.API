using Microsoft.EntityFrameworkCore;
using Web.Domain.Entities;
using Web.Domain.Interfaces;
using Web.Infrastructure.Data.Context;

namespace Web.Infrastructure.Repositories
{
    public class ImagemProdutoRepository : GenericRepository<ImagemProduto>, IImagemProdutoRepository
    {
        public ImagemProdutoRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ImagemProduto>> GetImagensByProdutoIdAsync(int produtoId)
        {
            return await _dbSet
                .Where(i => i.ProdutoId == produtoId)
                .OrderByDescending(i => i.Principal)
                .ToListAsync();
        }

        public async Task<ImagemProduto> GetImagemPrincipalByProdutoIdAsync(int produtoId)
        {
            return await _dbSet
                .Where(i => i.ProdutoId == produtoId && i.Principal)
                .FirstOrDefaultAsync();
        }
    }
}