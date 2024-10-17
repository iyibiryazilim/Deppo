using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.Views;

public partial class ProcurementSalesPackageProductListView : ContentPage
{
	private readonly ProcurementSalesPackageProductListViewModel _viewModel;
    public ProcurementSalesPackageProductListView(ProcurementSalesPackageProductListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.SearchText = searchBar;

    }

}