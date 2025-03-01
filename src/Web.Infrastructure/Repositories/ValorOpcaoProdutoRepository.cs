using Microsoft.EntityFrameworkCore;
using Web.Domain.Entities;
using Web.Domain.Interfaces;
using Web.Infrastructure.Data.Context;

namespace Web.Infrastructure.Repositories
{
    public class ValorOpcaoProdutoRepository : GenericRepository<ValorOpcaoProduto>, IValorOpcaoProdutoRepository
    {
        public ValorOpcaoProdutoRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<ValorOpcaoProduto>> GetAllByOpcaoIdAsync(int estabelecimentoId, int produtoId, int opcaoId)
        {
            return await _dbSet
                .Include(v => v.OpcaoProduto)
                .ThenInclude(o => o.Produto)
                .Where(v => v.OpcaoProduto.Id == opcaoId &&
                            v.OpcaoProduto.ProdutoId == produtoId &&
                            v.OpcaoProduto.Produto.EstabelecimentoId == estabelecimentoId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ValorOpcaoProduto> GetValorByIdAsync(int estabelecimentoId, int produtoId, int opcaoId, int valorId)
        {
            return await _dbSet
                .Include(v => v.OpcaoProduto)
                .ThenInclude(o => o.Produto)
                .FirstOrDefaultAsync(v =>
                    v.Id == valorId &&
                    v.OpcaoProdutoId == opcaoId &&
                    v.OpcaoProduto.ProdutoId == produtoId &&
                    v.OpcaoProduto.Produto.EstabelecimentoId == estabelecimentoId);
        }

        public async Task AddValorAsync(int estabelecimentoId, int produtoId, int opcaoId, ValorOpcaoProduto valor)
        {
            // Verifica se a opção existe e pertence ao produto e estabelecimento
            var opcao = await _context.OpcoesProduto
                .Include(o => o.Produto)
                .FirstOrDefaultAsync(o => o.Id == opcaoId && o.ProdutoId == produtoId && o.Produto.EstabelecimentoId == estabelecimentoId);

            if (opcao == null)
                throw new Exception("Opção não encontrada.");

            valor.OpcaoProdutoId = opcaoId;
            await _dbSet.AddAsync(valor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateValorAsync(int estabelecimentoId, int produtoId, int opcaoId, ValorOpcaoProduto valor)
        {
            var existing = await _dbSet
                .Include(v => v.OpcaoProduto)
                .ThenInclude(o => o.Produto)
                .FirstOrDefaultAsync(v =>
                    v.Id == valor.Id &&
                    v.OpcaoProdutoId == opcaoId &&
                    v.OpcaoProduto.ProdutoId == produtoId &&
                    v.OpcaoProduto.Produto.EstabelecimentoId == estabelecimentoId);

            if (existing == null)
                throw new Exception("Valor não encontrado ou não pertence à opção, produto e estabelecimento especificados.");

            _context.Entry(existing).CurrentValues.SetValues(valor);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveValorAsync(int estabelecimentoId, int produtoId, int opcaoId, int valorId)
        {
            var valor = await _dbSet
                .Include(v => v.OpcaoProduto)
                .ThenInclude(o => o.Produto)
                .FirstOrDefaultAsync(v =>
                    v.Id == valorId &&
                    v.OpcaoProdutoId == opcaoId &&
                    v.OpcaoProduto.ProdutoId == produtoId &&
                    v.OpcaoProduto.Produto.EstabelecimentoId == estabelecimentoId);

            if (valor == null)
                throw new Exception("Valor não encontrado ou não pertence à opção, produto e estabelecimento especificados.");

            _dbSet.Remove(valor);
            await _context.SaveChangesAsync();
        }
    }
}