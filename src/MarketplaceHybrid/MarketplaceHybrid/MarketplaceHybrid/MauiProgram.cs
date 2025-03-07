﻿using MarketplaceHybrid.Services;
using MarketplaceHybrid.Shared.Configurations;
using MarketplaceHybrid.Shared.Services;
using MarketplaceHybrid.Shared.Services.Interfaces;
using Microsoft.Extensions.Logging;
using MudBlazor;
using MudBlazor.Services;

namespace MarketplaceHybrid
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });
            // Adiciona os serviços do MudBlazor
            builder.Services.AddMudServices();
            // Add device-specific services used by the MarketplaceHybrid.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();

            builder.Services.AddMauiBlazorWebView();
            // Registra o HttpClient e o serviço de Estabelecimentos
            builder.Services.AddScoped<IEstabelecimentoService, EstabelecimentoService>();
            builder.Services.AddScoped<IProdutoService, ProdutoService>();
            builder.Services.AddScoped<ICarrinhoService, CarrinhoService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IHorarioFuncionamentoService, HorarioFuncionamentoService>();
            builder.Services.AddScoped<IEnderecoClienteService, EnderecoClienteService>();
            builder.Services.AddScoped<IPedidoService, PedidoService>();
            builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<ICategoriaService, CategoriaService>();

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
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<HttpClient>(sp =>
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri(ApiConstants.BaseUrl)
                };
                return client;
            });

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
