using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesDispatchProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesDispatchProcess.Views;

public partial class ReturnSalesDispatchWarehouseListView : ContentPage
{
    private readonly ReturnSalesDispatchWarehouseListViewModel _viewModel;

    public ReturnSalesDispatchWarehouseListView(ReturnSalesDispatchWarehouseListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}