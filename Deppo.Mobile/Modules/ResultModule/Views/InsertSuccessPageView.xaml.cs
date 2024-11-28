using Deppo.Mobile.Modules.ResultModule.ViewModels;

namespace Deppo.Mobile.Modules.ResultModule.Views;

public partial class InsertSuccessPageView : ContentPage
{
	private readonly InsertSuccessPageViewModel _viewModel;
	public InsertSuccessPageView(InsertSuccessPageViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}