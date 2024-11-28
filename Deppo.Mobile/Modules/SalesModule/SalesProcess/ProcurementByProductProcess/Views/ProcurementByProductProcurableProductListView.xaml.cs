using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.Views;

public partial class ProcurementByProductProcurableProductListView : ContentPage
{
	private readonly ProcurementByProductProcurableProductListViewModel _viewModel;
	public ProcurementByProductProcurableProductListView(ProcurementByProductProcurableProductListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
		_viewModel.CurrentPage = this;
		_viewModel.SearchText = searchBar;
	}
}