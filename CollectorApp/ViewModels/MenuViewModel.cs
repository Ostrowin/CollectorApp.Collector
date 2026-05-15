using CommunityToolkit.Mvvm.Input;
using CollectorApp.Services.Interfaces;

namespace CollectorApp.ViewModels;

public partial class MenuViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    public MenuViewModel(INavigationService navigationService)
    {
        Title = "Menu";
        _navigationService = navigationService;
    }
    [RelayCommand]
    private async Task GoToScannerAsync()
    {
        await _navigationService.GoToAsync("//scanner");
    }
}
