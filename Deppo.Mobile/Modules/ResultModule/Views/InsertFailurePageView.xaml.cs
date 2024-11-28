using Deppo.Mobile.Modules.ResultModule.ViewModels;

namespace Deppo.Mobile.Modules.ResultModule.Views;

public partial class InsertFailurePageView : ContentPage
{
	private readonly InsertFailurePageViewModel _viewModel;
	public InsertFailurePageView(InsertFailurePageViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}