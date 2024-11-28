using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.Views;

public partial class ProcurementByProductWarehouseListView : ContentPage
{
	private readonly ProcurementByProductWarehouseListViewModel _viewModel;
	public ProcurementByProductWarehouseListView(ProcurementByProductWarehouseListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}