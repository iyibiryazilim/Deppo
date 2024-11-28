using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.Views;

public partial class OutputProductSalesOrderProcessProductListView : ContentPage
{
	private readonly OutputProductSalesOrderProcessProductListViewModel _viewModel;
	public OutputProductSalesOrderProcessProductListView(OutputProductSalesOrderProcessProductListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
		_viewModel.ProductSearchText = searchBar;
    }
}