using MarketplaceHybrid.Shared.Services.Interfaces;
using MarketplaceHybrid.Web.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
// Adiciona os serviços do MudBlazor
builder.Services.AddMudServices();
// Adiciona os serviços do MudBlazor
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = true;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
});
// Add device-specific services used by the MarketplaceHybrid.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

await builder.Build().RunAsync();
