using MarketplaceHybrid.Shared.Configurations;
using MarketplaceHybrid.Shared.Services;
using MarketplaceHybrid.Shared.Services.Interfaces;
using MarketplaceHybrid.Web.Components;
using MarketplaceHybrid.Web.Services;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Add device-specific services used by the MarketplaceHybrid.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();
// Adiciona os serviços do MudBlazor
builder.Services.AddMudServices();
builder.Services.AddScoped<HttpClient>(sp =>
{
    var client = new HttpClient
    {
        BaseAddress = new Uri(ApiConstants.BaseUrl)
    };
    return client;
});

builder.Services.AddScoped<IEstabelecimentoService, EstabelecimentoService>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<ICarrinhoService,CarrinhoService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHorarioFuncionamentoService, HorarioFuncionamentoService>();
builder.Services.AddScoped<IEnderecoClienteService, EnderecoClienteService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        typeof(MarketplaceHybrid.Shared._Imports).Assembly,
        typeof(MarketplaceHybrid.Web.Client._Imports).Assembly);

app.Run();
