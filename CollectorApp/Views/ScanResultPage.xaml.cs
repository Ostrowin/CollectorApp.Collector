using CollectorApp.ViewModels;
using CollectorApp.Helpers;

namespace CollectorApp.Views;

public partial class ScanResultPage : BasePage
{
    public ScanResultPage() : this(ServiceLoctor.Get<ScanResultViewModel>()) { }

    public ScanResultPage(ScanResultViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}