using CollectorApp.Models;

namespace CollectorApp.Services.Interfaces;

public interface IAuthService
{
    Task<User?> LoginAsync(string name, string surname, string password);
    bool IsLoggedIn { get; }
    User? CurrentUser { get; }
    Task<bool> TryRestoreSessionAsync();
    Task LogoutAsync();
    Task<bool> ValidateSessionAsync();
}
