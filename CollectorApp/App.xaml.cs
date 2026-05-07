using Microsoft.Extensions.DependencyInjection;

namespace CollectorApp;

public partial class App : Application
{
    public App(AppShell shell)
    {
        InitializeComponent();
        MainPage = shell;
    }
}