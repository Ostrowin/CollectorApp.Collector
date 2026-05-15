using CollectorApp.Helpers;
using CollectorApp.ViewModels;

namespace CollectorApp.Views;

public partial class SplashLoadingPage : BasePage
{
    public SplashLoadingPage() : this(ServiceLoctor.Get<SplashLoadingViewModel>()) { }

    public SplashLoadingPage(SplashLoadingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ((SplashLoadingViewModel)BindingContext).InitializeAsync();
    }
}