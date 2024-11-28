using Deppo.Mobile.Modules.LoginModule.ViewModels;

namespace Deppo.Mobile.Modules.LoginModule.Views;

public partial class LoginView : ContentPage
{
	private readonly LoginViewModel _viewModel;
	public LoginView(LoginViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}