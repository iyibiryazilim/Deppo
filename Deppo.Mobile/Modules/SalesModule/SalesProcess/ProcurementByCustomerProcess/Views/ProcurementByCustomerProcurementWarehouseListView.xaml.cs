using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.Views;

public partial class ProcurementByCustomerProcurementWarehouseListView : ContentPage
{
	private readonly ProcurementByCustomerProcurementWarehouseListViewModel _viewModel;
	public ProcurementByCustomerProcurementWarehouseListView(ProcurementByCustomerProcurementWarehouseListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}