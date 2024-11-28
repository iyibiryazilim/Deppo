using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.Views;

public partial class ProcurementByCustomerListView : ContentPage
{
	private readonly ProcurementByCustomerListViewModel _viewModel;
	public ProcurementByCustomerListView(ProcurementByCustomerListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
		_viewModel.SearchText = searchBar;
	}
}