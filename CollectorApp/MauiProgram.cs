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

        services.AddHttpClient<IApiService, ApiService>(client =>
        {
#if DEBUG && ANDROID
        client.BaseAddress = new Uri("https://192.168.1.21:7037");
#else
            client.BaseAddress = new Uri("https://192.168.1.21:7037");
#endif
            client.Timeout = TimeSpan.FromSeconds(30);
        })
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
#if DEBUG && ANDROID
        return new SocketsHttpHandler
        {
            SslOptions = new System.Net.Security.SslClientAuthenticationOptions
            {
                RemoteCertificateValidationCallback = (sender, cert, chain, errors) => true
            }
        };
#else
        return new SocketsHttpHandler();
#endif
    })
    .AddStandardResilienceHandler(options =>
    {
        options.Retry.MaxRetryAttempts = 3;
        options.Retry.Delay = TimeSpan.FromSeconds(2);
        options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(60);
        options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(15);
    });

        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IDictionaryService, DictionaryService>();
    }

    private static void RegisterViewModels(IServiceCollection services)
    {
        services.AddTransient<SplashLoadingViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<MenuViewModel>();
        services.AddTransient<ScannerViewModel>();
        services.AddTransient<ScanResultViewModel>();
    }

    private static void RegisterViews(IServiceCollection services)
    {
        services.AddSingleton<AppShell>();
        services.AddTransient<SplashLoadingPage>();
        services.AddTransient<MenuPage>();
        services.AddTransient<LoginPage>();
        services.AddTransient<ScannerPage>();
        services.AddTransient<ScanResultPage>();
    }
}