using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.Views;

public partial class ProcurementByProductProcurementWarehouseListView : ContentPage
{
	private readonly ProcurementByProductProcurementWarehouseListViewModel _viewModel;
	public ProcurementByProductProcurementWarehouseListView(ProcurementByProductProcurementWarehouseListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}