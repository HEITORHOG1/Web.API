using Microsoft.EntityFrameworkCore;
using Web.Domain.DTOs;
using Web.Domain.DTOs.Produtos;
using Web.Domain.Entities;
using Web.Domain.Interfaces;
using Web.Infrastructure.Data.Context;

namespace Web.Infrastructure.Repositories
{
    public class ProdutoRepository : GenericRepository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<ProdutoDto>> GetAllByEstabelecimentoIdAsync(int estabelecimentoId)
        {
            return await _dbSet
                .Where(p => p.EstabelecimentoId == estabelecimentoId)
                .Include(p => p.Categoria)
                .Include(p => p.Opcoes)
                    .ThenInclude(o => o.Valores)
                .Include(p => p.Adicionais)
                .AsNoTracking()
                .Select(p => new ProdutoDto
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Descricao = p.Descricao,
                    Preco = p.Preco,
                    Imagem = p.Imagem,
                    Disponivel = p.Disponivel,
                    CategoriaId = p.CategoriaId,
                    NomeCategoria = p.Categoria.Nome,
                    DataCadastro = p.DataCadastro,
                    QuantidadeEmEstoque = p.QuantidadeEmEstoque,
                    CodigoDeBarras = p.CodigoDeBarras,
                    Opcoes = p.Opcoes.Select(o => new OpcaoProdutoDto
                    {
                        Id = o.Id,
                        ProdutoId = o.ProdutoId,
                        Nome = o.Nome,
                        Obrigatorio = o.Obrigatorio,
                        Valores = o.Valores.Select(v => new ValorOpcaoProdutoDto
                        {
                            Id = v.Id,
                            OpcaoProdutoId = v.OpcaoProdutoId,
                            Descricao = v.Descricao,
                            PrecoAdicional = v.PrecoAdicional
                        }).ToList()
                    }).ToList(),
                    Adicionais = p.Adicionais.Select(a => new AdicionalProdutoDto
                    {
                        Nome = a.Nome,
                        Preco = a.Preco
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<List<Produto>> GetByCategoriaIdAsync(int categoriaId)
        {
            return await _dbSet
                .Where(p => p.CategoriaId == categoriaId)
                .ToListAsync();
        }

        public async Task<Produto> GetProdutoByIdAsync(int estabelecimentoId, int produtoId)
        {
            return await _dbSet
                .Include(p => p.Categoria)
                .Include(p => p.Estabelecimento)
                .FirstOrDefaultAsync(p => p.Id == produtoId && p.EstabelecimentoId == estabelecimentoId);
        }

        public async Task<Produto?> GetProdutoAdiconaisByIdAsync(int estabelecimentoId, int produtoId)
        {
            return await _dbSet
                .Include(p => p.Opcoes)
                    .ThenInclude(op => op.Valores)
                .Include(p => p.Adicionais)
                .FirstOrDefaultAsync(p => p.EstabelecimentoId == estabelecimentoId && p.Id == produtoId);
        }

        public async Task AddProdutoAsync(int estabelecimentoId, Produto produto)
        {
            produto.EstabelecimentoId = estabelecimentoId;
            await _dbSet.AddAsync(produto);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProdutoAsync(int estabelecimentoId, Produto produto)
        {
            var existingProduto = await _dbSet.FirstOrDefaultAsync(p => p.Id == produto.Id && p.EstabelecimentoId == estabelecimentoId);
            if (existingProduto == null)
            {
                throw new Exception("Produto não encontrado ou não pertence ao estabelecimento especificado.");
            }
            _context.Entry(existingProduto).CurrentValues.SetValues(produto);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveProdutoAsync(int estabelecimentoId, int produtoId)
        {
            var produto = await _dbSet.FirstOrDefaultAsync(p => p.Id == produtoId && p.EstabelecimentoId == estabelecimentoId);
            if (produto == null)
            {
                throw new Exception("Produto não encontrado ou não pertence ao estabelecimento especificado.");
            }
            _dbSet.Remove(produto);
            await _context.SaveChangesAsync();
        }

        public async Task<Categoria> GetCategoriaByIdAsync(int estabelecimentoId, int categoriaId)
        {
            return await _context.Categorias.FirstOrDefaultAsync(c => c.Id == categoriaId && c.EstabelecimentoId == estabelecimentoId);
        }

        public async Task<List<Produto>> GetProdutosByCategoriaIdAsync(int estabelecimentoId, int categoriaId)
        {
            var query = _dbSet
                        .Where(p => p.EstabelecimentoId == estabelecimentoId && p.CategoriaId == categoriaId)
                        .Include(p => p.Categoria)
                        .AsQueryable();

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<bool> ExistsAsync(int estabelecimentoId, string nome)
        {
            return await _dbSet.AnyAsync(p => p.Nome == nome && p.EstabelecimentoId == estabelecimentoId);
        }

        public async Task<List<Produto>> ProdutoCadastradoNoEstabelecimentoAsync(int estabelecimentoId, string nomeProduto)
        {
            return await _dbSet
                .Where(p => p.EstabelecimentoId == estabelecimentoId && EF.Functions.Like(p.Nome, $"%{nomeProduto}%"))
                .ToListAsync();
        }
    }
}