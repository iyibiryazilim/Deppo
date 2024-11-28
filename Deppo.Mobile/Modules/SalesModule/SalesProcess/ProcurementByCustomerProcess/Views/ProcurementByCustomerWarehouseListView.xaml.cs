using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.Views;

public partial class ProcurementByCustomerWarehouseListView : ContentPage
{
	private readonly ProcurementByCustomerWarehouseListViewModel _viewModel;
	public ProcurementByCustomerWarehouseListView(ProcurementByCustomerWarehouseListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
		_viewModel.SearchText = locationSearchBar;
	}
}