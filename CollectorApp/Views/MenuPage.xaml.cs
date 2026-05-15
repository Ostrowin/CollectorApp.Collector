using CollectorApp.Helpers;
using CollectorApp.ViewModels;

namespace CollectorApp.Views;

public partial class MenuPage : BasePage
{
	public MenuPage() : this(ServiceLoctor.Get<MenuViewModel>()) { }
	public MenuPage(MenuViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}