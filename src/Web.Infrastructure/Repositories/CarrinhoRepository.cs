using Microsoft.EntityFrameworkCore;
using Web.Domain.Entities;
using Web.Domain.Interfaces;
using Web.Infrastructure.Data.Context;

namespace Web.Infrastructure.Repositories
{
    public class CarrinhoRepository : GenericRepository<CarrinhoItem>, ICarrinhoRepository
    {
        public CarrinhoRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CarrinhoItem>> GetCarrinhoItensByUsuarioIdAsync(string usuarioId)
        {
            return await _dbSet
                .Include(ci => ci.Produto)
                .Include(ci => ci.Estabelecimento)
                .Where(ci => ci.UsuarioId == usuarioId && ci.Status == Domain.Enums.StatusCarrinhoItem.Ativo)
                .ToListAsync();
        }

        public async Task<CarrinhoItem?> GetCarrinhoItemAsync(string usuarioId, int produtoId)
        {
            return await _dbSet
                .Include(ci => ci.Produto)
                .Include(ci => ci.Estabelecimento)
                .FirstOrDefaultAsync(ci => ci.UsuarioId == usuarioId && ci.ProdutoId == produtoId &&
                                            ci.Status == Domain.Enums.StatusCarrinhoItem.Ativo);
        }

        public async Task<CarrinhoItem?> GetItemAsync(string userId, int produtoId, int estabelecimentoId)
        {
            return await _context.CarrinhoItens
                        .FirstOrDefaultAsync(ci => ci.UsuarioId == userId &&
                                            ci.ProdutoId == produtoId &&
                                            ci.EstabelecimentoId == estabelecimentoId &&
                                            ci.Status == Domain.Enums.StatusCarrinhoItem.Ativo);
        }

        public async Task AtualizarItemAsync(CarrinhoItem carrinhoItem)
        {
            // Localizar o item no banco de dados
            var itemExistente = await _dbSet
                .FirstOrDefaultAsync(ci => ci.Id == carrinhoItem.Id && ci.UsuarioId == carrinhoItem.UsuarioId);

            // Atualizar propriedades
            itemExistente.Quantidade = carrinhoItem.Quantidade;
            itemExistente.Status = carrinhoItem.Status;
            itemExistente.PrecoUnitario = carrinhoItem.PrecoUnitario;

            // Atualizar no contexto
            _dbSet.Update(itemExistente);
            await _context.SaveChangesAsync();
        }

        public async Task AdicionarItensAoCarrinhoAsync(string usuarioId, List<CarrinhoItem> itens)
        {
            var executionStrategy = _context.Database.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        foreach (var item in itens)
                        {
                            // Verificar se o item já existe no carrinho
                            var carrinhoItemExistente = await GetItemAsync(usuarioId, item.ProdutoId, item.EstabelecimentoId);
                            if (carrinhoItemExistente != null)
                            {
                                // Incrementar a quantidade
                                carrinhoItemExistente.Quantidade += item.Quantidade;
                                await AtualizarItemAsync(carrinhoItemExistente);
                            }
                            else
                            {
                                // Adicionar novo item ao carrinho
                                item.UsuarioId = usuarioId;
                                item.DataAdicionado = DateTime.UtcNow;
                                await _context.CarrinhoItens.AddAsync(item);
                            }
                        }

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            });
        }

        public async Task AtualizarItensNoCarrinhoAsync(string usuarioId, List<CarrinhoItem> itens)
        {
            var executionStrategy = _context.Database.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        foreach (var item in itens)
                        {
                            // Verificar se o item existe no carrinho
                            var carrinhoItemExistente = await GetItemAsync(usuarioId, item.ProdutoId, item.EstabelecimentoId);
                            if (carrinhoItemExistente == null)
                            {
                                throw new Exception($"Item {item.ProdutoId} não encontrado no carrinho.");
                            }

                            // Atualizar a quantidade
                            carrinhoItemExistente.Quantidade = item.Quantidade;
                            await AtualizarItemAsync(carrinhoItemExistente);
                        }

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            });
        }

        public async Task RemoverItensDoCarrinhoAsync(string usuarioId, List<int> produtoIds)
        {
            var executionStrategy = _context.Database.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        foreach (var produtoId in produtoIds)
                        {
                            // Verificar se o item existe no carrinho
                            var carrinhoItem = await GetCarrinhoItemAsync(usuarioId, produtoId);
                            if (carrinhoItem != null)
                            {
                                _context.CarrinhoItens.Remove(carrinhoItem);
                            }
                        }

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            });
        }
    }
}