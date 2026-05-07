using CollectorApp.Helpers;
using CollectorApp.ViewModels;

namespace CollectorApp.Views;

public partial class LoginPage : BasePage
{
    public LoginPage() : this(ServiceLoctor.Get<LoginViewModel>()) { }
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}