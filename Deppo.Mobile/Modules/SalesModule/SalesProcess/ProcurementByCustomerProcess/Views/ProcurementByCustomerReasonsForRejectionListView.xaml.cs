using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.Views;

public partial class ProcurementByCustomerReasonsForRejectionListView : ContentPage
{
	private readonly ProcurementByCustomerReasonsForRejectionListViewModel _viewModel;
	public ProcurementByCustomerReasonsForRejectionListView(ProcurementByCustomerReasonsForRejectionListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}