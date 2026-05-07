using CommunityToolkit.Mvvm.ComponentModel;

namespace CollectorApp.ViewModels;

public abstract partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _IsBusy;

    [ObservableProperty]
    private string _title = string.Empty;

    public bool IsNotBusy => !IsBusy;
}
