using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.Views;

public partial class OutputProductSalesOrderProcessFormView : ContentPage
{
	private readonly OutputProductSalesOrderProcessFormViewModel _viewModel;
	public OutputProductSalesOrderProcessFormView(OutputProductSalesOrderProcessFormViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}