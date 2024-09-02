using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.Views;

public partial class OutputProductSalesOrderProcessWarehouseListView : ContentPage
{
	private readonly OutputProductSalesOrderProcessWarehouseListViewModel _viewModel;
	public OutputProductSalesOrderProcessWarehouseListView(OutputProductSalesOrderProcessWarehouseListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}
}