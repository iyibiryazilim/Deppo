using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.Views;

public partial class OutputProductSalesProcessWarehouseListView : ContentPage
{
	private readonly OutputProductSalesProcessWarehouseListViewModel _viewModel;
	public OutputProductSalesProcessWarehouseListView(OutputProductSalesProcessWarehouseListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}
}