using CollectorApp.Helpers;
using CollectorApp.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CollectorApp.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    [NotifyPropertyChangedFor(nameof(IsNameValid))]
    private string _name = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    [NotifyPropertyChangedFor(nameof(IsSurnameValid))]
    private string _surname = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    [NotifyPropertyChangedFor(nameof(IsPasswordValid))]
    private string _password = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string _errorMessage = string.Empty;

    public bool IsNameValid => Name.Length >= 2;
    public bool IsSurnameValid => Surname.Length >= 2;
    public bool IsPasswordValid => Password.Length >= 3;
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public bool IsDebugMode =>
#if DEBUG
    true;
#else
    false;
#endif

    public LoginViewModel(IAuthService authService, INavigationService navigationService)
    {
        Title = "Logowanie";
        _authService = authService;
        _navigationService = navigationService;
    }

    private bool CanLogin() =>
        IsNameValid &&
        IsSurnameValid &&
        IsPasswordValid &&
        !IsBusy;

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task LoginAsync()
    {
        IsBusy = true;
        ErrorMessage = string.Empty;

        //!!
        //await _navigationService.GoToAsync("//menu");

        try
        {
            var user = await _authService.LoginAsync(Name, Surname, Password);
            if (user is null)
            {
                ErrorMessage = "Nieprawidłowa nazwa użytkownika lub hasło.";
                return;
            }
            await _navigationService.GoToAsync("//menu");
        }
        catch (Exception ex)
        {
            ErrorMessage = "Błąd połączenia. Spróbuj ponownie.";
            AppLogger.Log($"LOGIN ERROR: {ex.GetType().Name}");
            AppLogger.Log($"MESSAGE: {ex.Message}");
            AppLogger.Log($"INNER: {ex.InnerException?.Message}");
            AppLogger.Log($"INNER2: {ex.InnerException?.InnerException?.Message}");
            AppLogger.Log($"STACK: {ex.StackTrace}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SkipLoginAsync()
    {
        await _navigationService.GoToAsync("//menu");
    }
}
