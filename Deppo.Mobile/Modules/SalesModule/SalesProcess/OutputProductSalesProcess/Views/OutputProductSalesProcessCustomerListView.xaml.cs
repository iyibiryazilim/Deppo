using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.Views;

public partial class OutputProductSalesProcessCustomerListView : ContentPage
{
	private readonly OutputProductSalesProcessCustomerListViewModel _viewModel;
	public OutputProductSalesProcessCustomerListView(OutputProductSalesProcessCustomerListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
		_viewModel.CurrentPage = this;
	}
}