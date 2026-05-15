using CollectorApp.Models;
using CollectorApp.Models.Api;
using CollectorApp.Services.Interfaces;


namespace CollectorApp.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IApiService _apiService;
    private User? _currentUser;

    private const string TokenKey = "auth_token";
    private const string TokenExpirationKey = "auth_token_expiration";
    private const string LoginKey = "auth_login";

    public bool IsLoggedIn => _currentUser != null && _currentUser.TokenExpiration > DateTime.Now;
    public User? CurrentUser => _currentUser;

    public AuthService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<User?> LoginAsync(string name, string surname, string password)
    {
        var response = await _apiService.LoginAsync(new LoginRequest(name, surname, password));
        
        var login = $"{name} {surname}";
        _currentUser = new User(login, response.Token, response.Expiration);
        
        _apiService.SetAuthToken(response.Token);
        
        await SaveToSecureStorageAsync(login, response.Token, response.Expiration);

        return _currentUser;
    }

    public async Task<bool> TryRestoreSessionAsync()
    {
        var login = await SecureStorage.Default.GetAsync(LoginKey);
        var token = await SecureStorage.Default.GetAsync(TokenKey);
        var expirationStr = await SecureStorage.Default.GetAsync(TokenExpirationKey);
        
        if (string.IsNullOrEmpty(login) || 
            string.IsNullOrEmpty(token) || 
            string.IsNullOrEmpty(expirationStr))
            return false;

        if (!DateTime.TryParse(expirationStr, out var expiration))
            return false;

        if (expiration <= DateTime.Now)
        {
            await ClearSecureStorageAsync();
            return false;
        }

        _currentUser = new User(login, token, expiration);
        _apiService.SetAuthToken(token);

        return true;
    }

    public async Task LogoutAsync()
    {
        _currentUser = null;
        _apiService.SetAuthToken(string.Empty);
        await ClearSecureStorageAsync();
    }

    private async Task SaveToSecureStorageAsync(string login, string token, DateTime expiration)
    {
        await SecureStorage.Default.SetAsync(LoginKey, login);
        await SecureStorage.Default.SetAsync(TokenKey, token);
        await SecureStorage.Default.SetAsync(TokenExpirationKey, expiration.ToString("o"));
    }

    private Task ClearSecureStorageAsync()
    {
        SecureStorage.Default.Remove(LoginKey);
        SecureStorage.Default.Remove(TokenKey);
        SecureStorage.Default.Remove(TokenExpirationKey);
        return Task.CompletedTask;
    }
}