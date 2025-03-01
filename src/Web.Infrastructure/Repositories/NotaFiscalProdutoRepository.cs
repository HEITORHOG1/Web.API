using Web.Domain.Entities;
using Web.Domain.Interfaces;
using Web.Infrastructure.Data.Context;

namespace Web.Infrastructure.Repositories
{
    public class NotaFiscalProdutoRepository : GenericRepository<NotaFiscalProduto>, INotaFiscalProdutoRepository
    {
        private readonly AppDbContext _context;

        public NotaFiscalProdutoRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}