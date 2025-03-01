using MarketplaceHybrid.Shared.Services.Interfaces;
using MarketplaceHybrid.Web.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
// Adiciona os serviços do MudBlazor
builder.Services.AddMudServices();

// Add device-specific services used by the MarketplaceHybrid.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

await builder.Build().RunAsync();
