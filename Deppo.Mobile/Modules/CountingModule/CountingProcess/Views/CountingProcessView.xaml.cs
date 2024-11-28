using Deppo.Mobile.Modules.CountingModule.CountingProcess.ViewModels;

namespace Deppo.Mobile.Modules.CountingModule.CountingProcess.Views;

public partial class CountingProcessView : ContentPage
{
	private readonly CountingProcessViewModel _viewModel;
	public CountingProcessView(CountingProcessViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}