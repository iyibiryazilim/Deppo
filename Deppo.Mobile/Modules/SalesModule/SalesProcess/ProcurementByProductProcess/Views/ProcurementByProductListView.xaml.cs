using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.Views;

public partial class ProcurementByProductListView : ContentPage
{
	private readonly ProcurementByProductListViewModel _viewModel;
	public ProcurementByProductListView(ProcurementByProductListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
		_viewModel.SearchText = searchBar;
	}
}