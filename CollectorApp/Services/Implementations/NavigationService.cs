using CollectorApp.Services.Interfaces;

namespace CollectorApp.Services.Implementations;

public class NavigationService : INavigationService
{
    public async Task GoToAsync(string route)
        => await Shell.Current.GoToAsync(route);

    public async Task GoToAsync(string route, Dictionary<string, object> parameters)
        => await Shell.Current.GoToAsync(route, parameters);

    public async Task GoBackAsync()
        => await Shell.Current.GoToAsync("..");
}