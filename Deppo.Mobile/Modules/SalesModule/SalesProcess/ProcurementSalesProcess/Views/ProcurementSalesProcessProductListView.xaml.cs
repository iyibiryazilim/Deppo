using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.Views;

public partial class ProcurementSalesProcessProductListView : ContentPage
{
	private readonly ProcurementSalesProcessProductListViewModel _viewModel;
    public ProcurementSalesProcessProductListView(ProcurementSalesProcessProductListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}