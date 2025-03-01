using Microsoft.EntityFrameworkCore;
using Web.Domain.Entities;
using Web.Domain.Interfaces;
using Web.Infrastructure.Data.Context;

namespace Web.Infrastructure.Repositories
{
    public class OpcaoProdutoRepository : GenericRepository<OpcaoProduto>, IOpcaoProdutoRepository
    {
        public OpcaoProdutoRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<OpcaoProduto>> GetAllByProdutoIdAsync(int estabelecimentoId, int produtoId)
        {
            return await _dbSet
                .Where(o => o.Produto.EstabelecimentoId == estabelecimentoId && o.ProdutoId == produtoId)
                .Include(o => o.Valores)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<OpcaoProduto> GetOpcaoByIdAsync(int estabelecimentoId, int produtoId, int opcaoId)
        {
            return await _dbSet
                .Include(o => o.Valores)
                .Include(o => o.Produto)
                .FirstOrDefaultAsync(o =>
                    o.Id == opcaoId &&
                    o.ProdutoId == produtoId &&
                    o.Produto.EstabelecimentoId == estabelecimentoId);
        }

        public async Task AddOpcaoAsync(int estabelecimentoId, int produtoId, OpcaoProduto opcao)
        {
            // Verifica se o produto existe e pertence ao estabelecimento
            var produto = await _context.Produtos.FirstOrDefaultAsync(p => p.Id == produtoId && p.EstabelecimentoId == estabelecimentoId);
            if (produto == null)
                throw new Exception("Produto não encontrado ou não pertence ao estabelecimento.");

            opcao.ProdutoId = produtoId;
            await _dbSet.AddAsync(opcao);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOpcaoAsync(int estabelecimentoId, int produtoId, OpcaoProduto opcao)
        {
            var existing = await _dbSet
                .Include(o => o.Produto)
                .FirstOrDefaultAsync(o =>
                    o.Id == opcao.Id &&
                    o.ProdutoId == produtoId &&
                    o.Produto.EstabelecimentoId == estabelecimentoId);

            if (existing == null)
                throw new Exception("Opção não encontrada ou não pertence ao produto e estabelecimento especificados.");

            _context.Entry(existing).CurrentValues.SetValues(opcao);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveOpcaoAsync(int estabelecimentoId, int produtoId, int opcaoId)
        {
            var opcao = await _dbSet
                .Include(o => o.Produto)
                .FirstOrDefaultAsync(o =>
                    o.Id == opcaoId &&
                    o.ProdutoId == produtoId &&
                    o.Produto.EstabelecimentoId == estabelecimentoId);

            if (opcao == null)
                throw new Exception("Opção não encontrada ou não pertence ao produto e estabelecimento especificados.");

            _dbSet.Remove(opcao);
            await _context.SaveChangesAsync();
        }

        public async Task ReplicarOpcoesParaProdutosDaCategoriaAsync(int categoriaId, int produtoOrigemId)
        {
            // Obter o produto de origem e suas opções
            var produtoOrigem = await _context.Produtos
                .Include(p => p.Opcoes)
                .ThenInclude(o => o.Valores)
                .FirstOrDefaultAsync(p => p.Id == produtoOrigemId);

            if (produtoOrigem == null)
                throw new Exception("Produto de origem não encontrado.");

            // Obter outros produtos da mesma categoria, exceto o produto de origem
            var produtosDestino = await _context.Produtos
                .Where(p => p.CategoriaId == categoriaId && p.Id != produtoOrigemId)
                .ToListAsync();

            if (!produtosDestino.Any())
                return;

            foreach (var produtoDestino in produtosDestino)
            {
                // Excluir as opções existentes para o produto de destino
                var opcoesExistentes = await _context.OpcoesProduto
                    .Where(o => o.ProdutoId == produtoDestino.Id)
                    .ToListAsync();

                if (opcoesExistentes.Any())
                {
                    _context.OpcoesProduto.RemoveRange(opcoesExistentes);
                }

                // Replicar as opções do produto de origem para o produto de destino
                foreach (var opcaoOrigem in produtoOrigem.Opcoes)
                {
                    var novaOpcao = new OpcaoProduto
                    {
                        ProdutoId = produtoDestino.Id,
                        Nome = opcaoOrigem.Nome,
                        Obrigatorio = opcaoOrigem.Obrigatorio,
                        Valores = opcaoOrigem.Valores.Select(v => new ValorOpcaoProduto
                        {
                            Descricao = v.Descricao,
                            PrecoAdicional = v.PrecoAdicional
                        }).ToList()
                    };

                    await _dbSet.AddAsync(novaOpcao);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}