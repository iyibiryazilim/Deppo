using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.Views;

public partial class ProcurementSalesProcessWarehouseListView : ContentPage
{
	private readonly ProcurementSalesProcessWarehouseListViewModel _viewModel;
    public ProcurementSalesProcessWarehouseListView(ProcurementSalesProcessWarehouseListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

}