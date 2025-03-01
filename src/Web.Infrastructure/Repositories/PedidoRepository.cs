using Microsoft.EntityFrameworkCore;
using Web.Domain.Entities;
using Web.Domain.Interfaces;
using Web.Infrastructure.Data.Context;

namespace Web.Infrastructure.Repositories
{
    public class PedidoRepository : GenericRepository<Pedido>, IPedidoRepository
    {
        private readonly AppDbContext _context;

        public PedidoRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddPedidoComTransacaoAsync(Pedido pedido, IEnumerable<ItemPedido> itens)
        {
            try
            {
                // Obtém a estratégia de execução
                var executionStrategy = _context.Database.CreateExecutionStrategy();

                // Executa todas as operações dentro da estratégia
                await executionStrategy.ExecuteAsync(async () =>
                {
                    // Inicia a transação
                    await using var transaction = await _context.Database.BeginTransactionAsync();
                    try
                    {
                        // Atualiza o estoque dos produtos
                        foreach (var item in itens)
                        {
                            var produto = await _context.Produtos.FindAsync(item.ProdutoId);

                            if (produto == null)
                                throw new Exception($"Produto com Id {item.ProdutoId} não foi encontrado!");

                            if (produto.QuantidadeEmEstoque < item.Quantidade)
                                throw new Exception($"Quantidade de estoque insuficiente para o produto {produto.Nome}");

                            produto.QuantidadeEmEstoque -= item.Quantidade;
                            _context.Produtos.Update(produto);
                        }

                        // Adiciona o pedido e os itens
                        await _context.Pedidos.AddAsync(pedido);
                        await _context.SaveChangesAsync();

                        // Confirma a transação
                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        // Faz rollback em caso de erro
                        await transaction.RollbackAsync();
                        throw;
                    }
                });
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        public async Task<IEnumerable<Pedido>> GetByUserIdAsync(string usuarioId)
        {
            return await _dbSet
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .Where(p => p.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Pedido>> GetByEstabelecimentoIdAsync(int estabelecimentoId)
        {
            return await _dbSet
                .Include(p => p.Itens)
                .Where(p => p.EstabelecimentoId == estabelecimentoId)
                .ToListAsync();
        }

        public async Task<Pedido> GetByExternalReferenceAsync(string externalReference)
        {
            return await _dbSet
                .Include(p => p.Itens)
                .FirstOrDefaultAsync(p => p.ExternalReference == externalReference);
        }
    }
}