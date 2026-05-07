using CollectorApp.Models.Api;
using CollectorApp.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CollectorApp.ViewModels;

public partial class ScannerViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IApiService _apiService;

    [ObservableProperty]
    private bool _isScannerEnabled = true;

    [ObservableProperty]
    private bool _hasTorch = false;

    public ScannerViewModel(INavigationService navigationService, IApiService apiService)
    {
        Title = "Skaner";
        _navigationService = navigationService;
        _apiService = apiService;
    }

    public async Task OnBarcodeDetectedAsync(string rawValue, string format)
    {
        if (!_isScannerEnabled)
            return;

        _isScannerEnabled = false;

        try
        {
            await _apiService.SaveBarcodeAsync(new BarcodeRequest(rawValue, format));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Barcode save error: {ex}");
        }

        var scanResult = new Models.ScanResult(rawValue, format, DateTime.Now);

        var navigationParams = new Dictionary<string, object>
        {
            { "ScanResult", scanResult }
        };

        await _navigationService.GoToAsync("//result", navigationParams);
    }

    [RelayCommand]
    private void ToggleTorch()
    {
        HasTorch = !HasTorch;
    }

    [RelayCommand]
    private void ResetScanner()
    {
        IsScannerEnabled = true;
    }
}