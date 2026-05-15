using CollectorApp.Services.Implementations;
using CollectorApp.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CollectorApp.ViewModels;

public partial class SplashLoadingViewModel : BaseViewModel
{
    private readonly IDictionaryService _dictionaryService;
    private readonly INavigationService _navigationService;
    private readonly IAuthService _authService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private string _statusMessage = "Inicjalizacja...";

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public SplashLoadingViewModel(IDictionaryService dictionaryService, INavigationService navigationService, IAuthService authService)
    {
        _dictionaryService = dictionaryService;
        _navigationService = navigationService;
        _authService = authService;
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

            StatusMessage = "Sprawdzanie sesji...";
            var sessionRestored = await _authService.TryRestoreSessionAsync();

            StatusMessage = "Gotowe!";
            await Task.Delay(500); // ToDoFix: Remove after testing

            if (sessionRestored)
                await _navigationService.GoToAsync("//menu");
            else
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
