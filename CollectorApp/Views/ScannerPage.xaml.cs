using CollectorApp.ViewModels;
using CollectorApp.Helpers;

namespace CollectorApp.Views;

public partial class ScannerPage : BasePage
{
	private readonly ScannerViewModel _viewModel;
	public ScannerPage() : this(ServiceLoctor.Get<ScannerViewModel>()) { }
    public ScannerPage(ScannerViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }
}