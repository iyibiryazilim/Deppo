using Deppo.Mobile.Modules.LoginModule.ViewModels;

namespace Deppo.Mobile.Modules.LoginModule.Views;

public partial class CompanyListView : ContentPage
{
	CompanyListViewModel _viewModel;
	public CompanyListView(CompanyListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}
}