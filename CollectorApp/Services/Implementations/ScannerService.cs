using CollectorApp.Helpers;
using CollectorApp.Models;
using CollectorApp.Models.Api;
using CollectorApp.Services.Interfaces;
using CommunityToolkit.Mvvm.Messaging;

namespace CollectorApp.Services.Implementations;

public class ScannerService : IScannerService, IRecipient<BarcodeScannedMessage>
{
    private readonly IMessenger _messenger;
    private readonly IApiService _apiService;
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;
    private bool _isProcessing = false;

    public ScannerService(
        IMessenger messenger,
        IApiService apiService,
        IAuthService authService,
        INavigationService navigationService)
    {
        _messenger = messenger;
        _apiService = apiService;
        _navigationService = navigationService;
        _authService = authService;
    }

    public void Initialize()
    {
        _messenger.RegisterAll(this);
        AppLogger.Info("ScannerService initialized and listening for barcodes");
    }

    public async void Receive(BarcodeScannedMessage message)
    {
        if (_isProcessing)
        {
            AppLogger.Warning("ScannerService - already processing, skipping");
            return;
        }

        _isProcessing = true;
        AppLogger.Info($"ScannerService received | Value: {message.Value} | Format: {message.Format}");

        try
        {
            var sessionValid = await _authService.ValidateSessionAsync();
            if (!sessionValid) {
                AppLogger.Warning("ScannerService - invalid session, skipping barcode processing");
                _messenger.Send(new SessionExpiredMessage());
                return;
            }

            _ = _apiService.SaveBarcodeAsync(new BarcodeRequest(message.Value, message.Format));

            var scanResult = new ScanResult(message.Value, message.Format, DateTime.Now);

            var navigationParams = new Dictionary<string, object>
        {
            { "ScanResult", scanResult }
        };

            await _navigationService.GoToAsync("//result", navigationParams);
            AppLogger.Info("ScannerService - navigation to result completed");
        }
        catch (Exception ex)
        {
            AppLogger.Error("ScannerService - navigation error", ex);
        }
        finally
        {
            _isProcessing = false;
        }
    }
}