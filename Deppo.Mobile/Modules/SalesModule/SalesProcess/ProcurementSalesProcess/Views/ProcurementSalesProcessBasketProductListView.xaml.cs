using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.Views;

public partial class ProcurementSalesProcessBasketProductListView : ContentPage
{
	private readonly ProcurementSalesProcessBasketProductListViewModel _viewModel;
    public ProcurementSalesProcessBasketProductListView(ProcurementSalesProcessBasketProductListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.SearchText = searchBar;

    }
}