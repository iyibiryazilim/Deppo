using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.Views;

public partial class OutputProductSalesProcessFormView : ContentPage
{
	private readonly OutputProductSalesProcessFormViewModel _viewModel;
	public OutputProductSalesProcessFormView(OutputProductSalesProcessFormViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;

	}
}