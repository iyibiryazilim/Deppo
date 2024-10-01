using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.Views;

public partial class OutputProductSalesOrderProcessCustomerListView : ContentPage
{
	private readonly OutputProductSalesOrderProcessCustomerListViewModel _viewModel;
	public OutputProductSalesOrderProcessCustomerListView(OutputProductSalesOrderProcessCustomerListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
		_viewModel.CurrentPage = this;
		_viewModel.SearchText = searchBar;
    }
}