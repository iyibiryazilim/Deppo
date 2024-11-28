using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.Views;

public partial class ProcurementByProductReasonsForRejectionListView : ContentPage
{
	private readonly ProcurementByProductReasonsForRejectionListViewModel _viewModel;
	public ProcurementByProductReasonsForRejectionListView(ProcurementByProductReasonsForRejectionListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}