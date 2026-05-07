using CollectorApp.Models;
using CollectorApp.Models.Api;
using CollectorApp.Services.Interfaces;


namespace CollectorApp.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IApiService _apiService;
    private User? _currentUser;

    public bool IsLoggedIn => _currentUser != null;
    public User? CurrentUser => _currentUser;

    public AuthService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        var response = await _apiService.LoginAsync(new LoginRequest(email, password));

        _currentUser = new User(email, response.Token, response.Expiration);

        // Ustaw token dla kolejnych requestów
        _apiService.SetAuthToken(response.Token);

        return _currentUser;
    }

    public async Task LogoutAsync()
    {
        _currentUser = null;
        _apiService.SetAuthToken(string.Empty);
        await Task.CompletedTask;
    }

}