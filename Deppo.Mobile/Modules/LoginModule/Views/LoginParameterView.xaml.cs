using Deppo.Mobile.Modules.LoginModule.ViewModels;

namespace Deppo.Mobile.Modules.LoginModule.Views;

public partial class LoginParameterView : ContentPage
{
	private readonly LoginParameterViewModel _viewModel;
	public LoginParameterView(LoginParameterViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}