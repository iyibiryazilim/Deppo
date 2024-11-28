using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByLocationProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByLocationProcess.Views;

public partial class ProcurementByLocationWarehouseLocationListView : ContentPage
{
	private readonly ProcurementByLocationWarehouseLocationListViewModel _viewModel;
	public ProcurementByLocationWarehouseLocationListView(ProcurementByLocationWarehouseLocationListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.SearchText = searchBar;
	}
}