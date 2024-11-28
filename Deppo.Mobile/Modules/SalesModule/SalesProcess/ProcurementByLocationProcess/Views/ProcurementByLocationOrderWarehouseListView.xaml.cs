using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByLocationProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByLocationProcess.Views;

public partial class ProcurementByLocationOrderWarehouseListView : ContentPage
{
	private readonly ProcurementByLocationOrderWarehouseListViewModel _viewModel;
	public ProcurementByLocationOrderWarehouseListView(ProcurementByLocationOrderWarehouseListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}