using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CollectorApp.Services.Interfaces;
using CollectorApp.Models;

namespace CollectorApp.ViewModels;

[QueryProperty(nameof(ScanResult), "ScanResult")]
public partial class ScanResultViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FormattedDate))]
    [NotifyPropertyChangedFor(nameof(FormattedFormat))]
    private Models.ScanResult? _scanResult;

    public string FormattedDate => ScanResult is not null
        ? ScanResult.ScannedAt.ToString("dd.MM.yyyy HH:mm:ss")
        : string.Empty;

    public string FormattedFormat => ScanResult is not null
        ? ScanResult.BarcodeFormat.ToString()
        : string.Empty;

    public ScanResultViewModel(INavigationService navigationService)
    {
        Title = "Wynik skanowania";
        _navigationService = navigationService;
    }

    [RelayCommand]
    private async Task ScanAgainAsync()
    {
        await _navigationService.GoToAsync("//menu");
    }

    [RelayCommand]
    private async Task GenerateDocumentAsync()
    {
        if (_scanResult is null)
            return;

        var paramters = new Dictionary<string, object>
        {
            { "ScanResult", _scanResult }
        };

        //await _navigationService.GoToAsync("//document", paramters);
    }

    [RelayCommand]
    private async Task CopyToClipBoardAsync()
    {
        if (_scanResult is null)
            return;
        await Clipboard.Default.SetTextAsync(_scanResult.RawValue);
    }
}
