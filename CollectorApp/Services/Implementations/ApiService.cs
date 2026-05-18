using System.Net.Http.Headers;
using System.Net.Http.Json;
using CollectorApp.Helpers;
using CollectorApp.Models.Api;
using CollectorApp.Services.Interfaces;

namespace CollectorApp.Services.Implementations;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        AppLogger.Info($"LoginAsync started");
        var response = await _httpClient.PostAsJsonAsync("/api/Auth/login", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        AppLogger.Info("LoginAsync success");
        return result!;
    }

    public async Task<BarcodeResponse?> SaveBarcodeAsync(BarcodeRequest request)
    {
        AppLogger.Info($"SaveBarcodeAsync | Value: {request.Value} | Format: {request.Format}");
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Barcodes", request);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<BarcodeResponse>();
            AppLogger.Info($"SaveBarcodeAsync success | Id: {result?.Id}");
            return result;
        }
        catch (Exception ex)
        {
            AppLogger.Error("SaveBarcodeAsync failed", ex);
            return null;
        }
    }

    public void SetAuthToken(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }
}