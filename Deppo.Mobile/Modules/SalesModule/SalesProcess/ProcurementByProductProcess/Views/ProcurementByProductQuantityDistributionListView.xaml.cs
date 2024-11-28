using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.Views;

public partial class ProcurementByProductQuantityDistributionListView : ContentPage
{
	private readonly ProcurementByProductQuantityDistributionListViewModel _viewModel;
	public ProcurementByProductQuantityDistributionListView(ProcurementByProductQuantityDistributionListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
		_viewModel.CurrentPage = this;
	}
}