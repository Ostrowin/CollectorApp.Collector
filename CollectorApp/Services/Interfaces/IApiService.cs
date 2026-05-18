using CollectorApp.Models.Api;

namespace CollectorApp.Services.Interfaces;

public interface IApiService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<BarcodeResponse?> SaveBarcodeAsync(BarcodeRequest request);
    void SetAuthToken(string token);
}