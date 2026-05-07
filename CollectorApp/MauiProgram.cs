using CollectorApp.Services.Implementations;
using CollectorApp.Services.Interfaces;
using CollectorApp.ViewModels;
using CollectorApp.Views;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using ZXing.Net.Maui.Controls;

namespace CollectorApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseBarcodeReader()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        // Register services
        RegisterServices(builder.Services);
        // Register view models
        RegisterViewModels(builder.Services);
        // Register views
        RegisterViews(builder.Services);

#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }

    private static void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);

        services.AddSingleton<HttpClient>(_ =>
        {
#if DEBUG && ANDROID
            var handler = new SocketsHttpHandler
            {
                SslOptions = new System.Net.Security.SslClientAuthenticationOptions
                {
                    RemoteCertificateValidationCallback = (sender, cert, chain, errors) => true
                }
            };
            return new HttpClient(handler)
            {
                BaseAddress = new Uri("https://192.168.1.21:7037")
            };
#else
    return new HttpClient
    {
        BaseAddress = new Uri("https://192.168.1.21:7037")
    };
#endif
        });

        services.AddSingleton<ApiService>();
        services.AddSingleton<IApiService, ApiService>();
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<INavigationService, NavigationService>();
    }

    private static void RegisterViewModels(IServiceCollection services)
    {
        services.AddTransient<LoginViewModel>();
        services.AddTransient<ScannerViewModel>();
        services.AddTransient<ScanResultViewModel>();
    }

    private static void RegisterViews(IServiceCollection services)
    {
        services.AddSingleton<AppShell>();
        services.AddTransient<LoginPage>();
        services.AddTransient<ScannerPage>();
        services.AddTransient<ScanResultPage>();
    }
}