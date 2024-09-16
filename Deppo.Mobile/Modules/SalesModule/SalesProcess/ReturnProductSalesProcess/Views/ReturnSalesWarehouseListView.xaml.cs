using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesProcess.Views;

public partial class ReturnSalesWarehouseListView : ContentPage
{
    private readonly ReturnSalesWarehouseListViewModel _viewModel;

    public ReturnSalesWarehouseListView(ReturnSalesWarehouseListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}