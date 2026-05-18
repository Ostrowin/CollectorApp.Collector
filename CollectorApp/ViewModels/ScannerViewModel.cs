using CollectorApp.Helpers;
using CollectorApp.Models;
using CollectorApp.Models.Api;
using CollectorApp.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CollectorApp.ViewModels;

public partial class ScannerViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _statusMessage = "";

    [ObservableProperty]
    private bool _isProcessing = false;

    public ScannerViewModel()
    {
        Title = "Skaner";
    }


    [RelayCommand]
    private void ResetScanner()
    {
        IsProcessing = false;
        StatusMessage = "Przyłóż kod kreskowy do skanera";
    }
}