using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.Views;

public partial class ProcurementSalesProcessCustomerListView : ContentPage
{
	private readonly ProcurementSalesProcessCustomerListViewModel _viewModel;
    public ProcurementSalesProcessCustomerListView(ProcurementSalesProcessCustomerListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.SearchText = searchBar;
    }
}