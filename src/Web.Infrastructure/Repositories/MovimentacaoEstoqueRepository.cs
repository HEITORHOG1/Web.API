using Microsoft.EntityFrameworkCore;
using Web.Domain.Entities;
using Web.Domain.Interfaces;
using Web.Domain.Paginacao;
using Web.Infrastructure.Data.Context;

namespace Web.Infrastructure.Repositories
{
    public class MovimentacaoEstoqueRepository : GenericRepository<MovimentacaoEstoque>, IMovimentacaoEstoqueRepository
    {
        public MovimentacaoEstoqueRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<MovimentacaoEstoque>> GetByProdutoIdAsync(int produtoId, PaginationParameters paginationParameters)
        {
            return await GetPagedAsync(m => m.ProdutoId == produtoId, paginationParameters);
        }

        public async Task<MovimentacaoEstoque?> GetUltimaMovimentacaoAsync(int produtoId)
        {
            return await _context.MovimentacoesEstoque
                .Where(m => m.ProdutoId == produtoId)
                .OrderByDescending(m => m.DataMovimentacao)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<MovimentacaoEstoque>> GetMovimentacoesByEstabelecimentoIdAsync(int estabelecimentoId)
        {
            return await _dbSet
                .Include(me => me.Produto)
                .Where(me => me.EstabelecimentoId == estabelecimentoId)
                .ToListAsync();
        }
    }
}