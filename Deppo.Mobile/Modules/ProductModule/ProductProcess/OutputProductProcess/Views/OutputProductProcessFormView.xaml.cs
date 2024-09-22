using Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.Views;

public partial class OutputProductProcessFormView : ContentPage
{
	private OutputProductProcessFormViewModel _viewModel;
	public OutputProductProcessFormView(OutputProductProcessFormViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}