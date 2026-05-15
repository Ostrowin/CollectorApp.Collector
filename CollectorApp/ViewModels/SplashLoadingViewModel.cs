using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CollectorApp.Services.Interfaces;

namespace CollectorApp.ViewModels;

public partial class SplashLoadingViewModel : BaseViewModel
{
    private readonly IDictionaryService _dictionaryService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private string _statusMessage = "Inicjalizacja...";

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public SplashLoadingViewModel(IDictionaryService dictionaryService, INavigationService navigationService)
    {
        _dictionaryService = dictionaryService;
        _navigationService = navigationService;
    }

    public async Task InitializeAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            StatusMessage = "Ładowanie słowników...";
            await _dictionaryService.RefreshAllAsync();
            StatusMessage = "Gotowe!";
            await Task.Delay(500);
            await _navigationService.GoToAsync("//login");
        }
        catch (InvalidOperationException ex)
        {
            ErrorMessage = $"Błąd podczas inicjalizacji: {ex.Message}";
        }
        catch (Exception ex)
        {
            ErrorMessage = "Nieoczekiwany błąd. Spróbuj ponownie.";
            System.Diagnostics.Debug.WriteLine($"Splash error: {ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RetryAsync() => await InitializeAsync();
}
