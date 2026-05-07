using CollectorApp.Models;

namespace CollectorApp.Services.Interfaces;

public interface IAuthService
{
    Task<User?> LoginAsync(string email, string password);
    Task LogoutAsync();
    bool IsLoggedIn { get; }
    User? CurrentUser { get; }
}
