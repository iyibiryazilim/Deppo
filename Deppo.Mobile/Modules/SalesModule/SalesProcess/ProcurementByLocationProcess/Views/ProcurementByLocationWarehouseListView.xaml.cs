using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByLocationProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByLocationProcess.Views;

public partial class ProcurementByLocationWarehouseListView : ContentPage
{
	private readonly ProcurementByLocationWarehouseListViewModel _viewModel;
	public ProcurementByLocationWarehouseListView(ProcurementByLocationWarehouseListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}