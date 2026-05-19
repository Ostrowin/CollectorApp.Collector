using CollectorApp.Helpers;
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
        AppLogger.Info($"LoginAsync started for: {name} {surname}");
        var response = await _apiService.LoginAsync(new LoginRequest(name, surname, password));
        
        var login = $"{name} {surname}";
        _currentUser = new User(login, response.Token, response.Expiration);
        
        _apiService.SetAuthToken(response.Token);
        
        await SaveToSecureStorageAsync(login, response.Token, response.Expiration);
        AppLogger.Info("LoginAsync success");

        return _currentUser;
    }

    public async Task<bool> TryRestoreSessionAsync()
    {
        AppLogger.Info("TryRestoreSessionAsync started");
        var login = await SecureStorage.Default.GetAsync(LoginKey);
        var token = await SecureStorage.Default.GetAsync(TokenKey);
        var expirationStr = await SecureStorage.Default.GetAsync(TokenExpirationKey);
        
        if (string.IsNullOrEmpty(login) || 
            string.IsNullOrEmpty(token) ||
            string.IsNullOrEmpty(expirationStr))
        {
            AppLogger.Warning("TryRestoreSessionAsync - no token found");
            return false;
        }

        if (!DateTime.TryParse(expirationStr, out var expiration))
            return false;

        if (expiration <= DateTime.Now)
        {
            AppLogger.Warning("TryRestoreSessionAsync - token expired, clearing storage");
            await ClearSecureStorageAsync();
            return false;
        }

        _currentUser = new User(login, token, expiration);
        _apiService.SetAuthToken(token);
        AppLogger.Info($"Session restored for: {login}");

        return true;
    }

    public async Task LogoutAsync()
    {
        _currentUser = null;
        _apiService.SetAuthToken(string.Empty);
        await ClearSecureStorageAsync();
        AppLogger.Info("LogoutAsync completed");
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

    public Task<bool> ValidateSessionAsync()
    {
        if (_currentUser == null)
        {
            AppLogger.Warning("ValidateSessionAsync - no current user");
            return Task.FromResult(false);
        }

        if (_currentUser.TokenExpiration <= DateTime.Now)
        {
            AppLogger.Warning("ValidateSessionAsync - token expired");
            _ = LogoutAsync();
            return Task.FromResult(false);
        }

        AppLogger.Info($"ValidateSession - ok, expires: {_currentUser.TokenExpiration:HH:mm:ss}");
        return Task.FromResult(true);
    }
}