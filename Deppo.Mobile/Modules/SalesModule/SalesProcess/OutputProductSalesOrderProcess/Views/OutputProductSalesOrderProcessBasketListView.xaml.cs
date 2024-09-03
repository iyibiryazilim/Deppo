using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.Views;

public partial class OutputProductSalesOrderProcessBasketListView : ContentPage
{
	private readonly OutputProductSalesOrderProcessBasketListViewModel _viewModel;
	public OutputProductSalesOrderProcessBasketListView(OutputProductSalesOrderProcessBasketListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}