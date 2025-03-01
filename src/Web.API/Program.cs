using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;
using Web.API.Configurations;
using Web.API.Extensions;
using Web.API.Middlewares;
using Web.API.Validators;
using Web.Application.Validators;
using Web.Domain.DTOs;
using Web.Domain.DTOs.MercadoPago;

var builder = WebApplication.CreateBuilder(args);
IdentityModelEventSource.ShowPII = true;
// Adiciona os defaults do Aspire (health checks, telemetria, etc)
builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
// Configuração de logs específica para produção

Log.Logger = new LoggerConfiguration()
    .WriteTo.File($"{AppContext.BaseDirectory}/Logs/webapi-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Console()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

// Configuração do OpenTelemetry para produção
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics.AddAspNetCoreInstrumentation()
               .AddHttpClientInstrumentation()
               .AddRuntimeInstrumentation();
    })
    .WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation()
               .AddHttpClientInstrumentation();
    });

// Adicionando serviços à aplicação
builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureIdentity();
builder.Services.ConfigureAuthorization();
builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.ConfigureSwagger();
builder.Services.AddCustomServices();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors();
builder.Services.Configure<MercadoPagoSettings>(builder.Configuration.GetSection("MercadoPago"));
builder.Services.AddHttpContextAccessor();
// Configuração do Swagger
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Apenas uma configuração de ReferenceHandler é necessária
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;

        // Ignora propriedades com valor null
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;

        // Formata o JSON com indentação
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Adiciona o FluentValidation
builder.Services.AddControllers()
    .AddFluentValidation(config =>
    {
        config.RegisterValidatorsFromAssemblyContaining<CategoriaValidator>();
        config.RegisterValidatorsFromAssemblyContaining<EstabelecimentoValidator>();
        config.RegisterValidatorsFromAssemblyContaining<ProdutoValidator>();
        config.RegisterValidatorsFromAssemblyContaining<UsuarioEstabelecimentoValidator>();
        config.RegisterValidatorsFromAssemblyContaining<RegisterModelValidator>();
        config.RegisterValidatorsFromAssemblyContaining<RegisterFuncionarioModelValidator>();
        config.RegisterValidatorsFromAssemblyContaining<LoginModelValidator>();
        config.RegisterValidatorsFromAssemblyContaining<ChangePasswordModelValidator>();
        config.RegisterValidatorsFromAssemblyContaining<UsuarioEstabelecimentoValidator>();
        config.RegisterValidatorsFromAssemblyContaining<ForgotPasswordModelValidator>();
        config.RegisterValidatorsFromAssemblyContaining<ResetPasswordModelValidator>();
        config.RegisterValidatorsFromAssemblyContaining<UpdateUserModelValidator>();
        config.RegisterValidatorsFromAssemblyContaining<FornecedorCreateDtoValidator>();
        config.RegisterValidatorsFromAssemblyContaining<FornecedorUpdateDtoValidator>();
        config.RegisterValidatorsFromAssemblyContaining<CreateEstabelecimentoDtoValidator>();
        config.RegisterValidatorsFromAssemblyContaining<CarrinhoItemValidator>();
        config.RegisterValidatorsFromAssemblyContaining<EntregaValidator>();
        config.RegisterValidatorsFromAssemblyContaining<PedidoValidator>();
        config.RegisterValidatorsFromAssemblyContaining<ItemPedidoValidator>();
        config.RegisterValidatorsFromAssemblyContaining<MovimentacaoEstoqueValidator>();
        config.RegisterValidatorsFromAssemblyContaining<NotaFiscalProdutoValidator>();
        config.RegisterValidatorsFromAssemblyContaining<NotaFiscalValidator>();
        config.RegisterValidatorsFromAssemblyContaining<CarrinhoItemDtoValidator>();
        config.RegisterValidatorsFromAssemblyContaining<AtribuirEntregaDtoValidator>();
        config.RegisterValidatorsFromAssemblyContaining<AdicionalProdutoDtoValidator>();
        config.RegisterValidatorsFromAssemblyContaining<ProdutoUpdateDtoValidator>();
        config.RegisterValidatorsFromAssemblyContaining<ProdutoCreateDtoValidator>();
        config.RegisterValidatorsFromAssemblyContaining<NotaFiscalProdutoDtoValidator>();
        config.RegisterValidatorsFromAssemblyContaining<NotaFiscalDtoValidator>();
        config.RegisterValidatorsFromAssemblyContaining<CategoriaUpdateDtoValidator>();
        config.RegisterValidatorsFromAssemblyContaining<CategoriaCreateDtoValidator>();
    });

// Versionamento da API
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
});

var app = builder.Build();

// Configuração dos endpoints de health check
app.MapDefaultEndpoints();

// Configuração do redirecionamento da rota raiz

app.UseMiddleware<ExceptionMiddleware>();
app.UseApiVersioning();
app.ConfigureMiddleware();

app.Run();

public partial class Program
{ }