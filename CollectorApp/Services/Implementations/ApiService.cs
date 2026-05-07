using System.Net.Http.Headers;
using System.Net.Http.Json;
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
        var response = await _httpClient.PostAsJsonAsync("/api/Auth/login", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return result!;
    }

    public async Task<BarcodeResponse> SaveBarcodeAsync(BarcodeRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/Barcodes", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<BarcodeResponse>();
        return result!;
    }

    public void SetAuthToken(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }
}