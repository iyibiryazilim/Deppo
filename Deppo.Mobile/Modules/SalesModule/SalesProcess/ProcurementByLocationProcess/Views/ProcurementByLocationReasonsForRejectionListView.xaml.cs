using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByLocationProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByLocationProcess.Views;

public partial class ProcurementByLocationReasonsForRejectionListView : ContentPage
{
	private readonly ProcurementByLocationReasonsForRejectionListViewModel _viewModel;
	public ProcurementByLocationReasonsForRejectionListView(ProcurementByLocationReasonsForRejectionListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}