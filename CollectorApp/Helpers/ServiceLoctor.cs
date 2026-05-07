namespace CollectorApp.Helpers;

public static class ServiceLoctor
{
    public static TService Get<TService>() where TService : class 
        => Current.GetRequiredService<TService>();

    public static IServiceProvider Current
        => IPlatformApplication.Current!.Services;
}
