using FluentValidation;
using Web.API.Services;
using Web.Application.Interfaces;
using Web.Application.Mappers;
using Web.Application.Services;
using Web.Application.Validators;
using Web.Domain.Entities;
using Web.Domain.Interfaces;
using Web.Infrastructure.Repositories;

namespace Web.API.Configurations
{
    /// <summary>
    /// Configuração de injeção de dependência.
    /// </summary>
    public static class DependencyInjectionConfig
    {
        /// <summary>
        /// Adiciona as dependências customizadas.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            // Registre todas as dependências aqui
            services.AddScoped<TokenService>();

            // Registro dos repositórios
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<IEstabelecimentoRepository, EstabelecimentoRepository>();
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            services.AddScoped<IUsuarioEstabelecimentoRepository, UsuarioEstabelecimentoRepository>();
            services.AddScoped<IErrorLogRepository, ErrorLogRepository>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IMovimentacaoEstoqueRepository, MovimentacaoEstoqueRepository>();
            services.AddScoped<IPedidoRepository, PedidoRepository>();
            services.AddScoped<ICarrinhoRepository, CarrinhoRepository>();
            services.AddScoped<IEntregadorRepository, EntregadorRepository>();
            services.AddScoped<IEntregaRepository, EntregaRepository>();
            services.AddScoped<INotaFiscalRepository, NotaFiscalRepository>();
            services.AddScoped<INotaFiscalProdutoRepository, NotaFiscalProdutoRepository>();
            services.AddScoped<IFornecedorRepository, FornecedorRepository>();
            services.AddScoped<IImagemProdutoRepository, ImagemProdutoRepository>();
            services.AddScoped<IValidator<ImagemProduto>, ImagemProdutoValidator>();
            services.AddScoped<IOpcaoProdutoRepository, OpcaoProdutoRepository>();
            services.AddScoped<IValorOpcaoProdutoRepository, ValorOpcaoProdutoRepository>();
            services.AddScoped<IHorarioFuncionamentoRepository, HorarioFuncionamentoRepository>();
            services.AddScoped<IEnderecoClienteRepository, EnderecoClienteRepository>();


            // Registro dos serviços
            services.AddScoped<ICategoriaService, CategoriaService>();
            services.AddScoped<IEstabelecimentoService, EstabelecimentoService>();
            services.AddScoped<IProdutoService, ProdutoService>();
            services.AddScoped<IUsuarioEstabelecimentoService, UsuarioEstabelecimentoService>();
            services.AddScoped<IErrorLogService, ErrorLogService>();
            services.AddScoped<IMovimentacaoEstoqueService, MovimentacaoEstoqueService>();
            services.AddScoped<IPagamentoService, PagamentoService>();
            services.AddScoped<IPedidoService, PedidoService>();
            services.AddScoped<ICarrinhoService, CarrinhoService>();
            services.AddScoped<IEntregaService, EntregaService>();
            services.AddScoped<IEntregadorService, EntregadorService>();
            services.AddScoped<INotaFiscalService, NotaFiscalService>();
            services.AddScoped<IFornecedorService, FornecedorService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IImageUploadService, ImageUploadService>();
            services.AddScoped<IImagemProdutoService, ImagemProdutoService>();
            services.AddScoped<IOpcaoProdutoService, OpcaoProdutoService>();
            services.AddScoped<IValorOpcaoProdutoService, ValorOpcaoProdutoService>();
            services.AddHttpClient<IGeocodingService, GeocodingService>();
            services.AddScoped<IHorarioFuncionamentoService, HorarioFuncionamentoService>();
            services.AddTransient<IMercadoPagoService, MercadoPagoService>();
            services.AddScoped<IEnderecoClienteService, EnderecoClienteService>();
            services.AddTransient<IMercadoPagoService, MercadoPagoService>();

            services.AddAutoMapper(typeof(CategoriaMapper));
            services.AddAutoMapper(typeof(EnderecoClienteMapper));
            return services;
        }
    }
}