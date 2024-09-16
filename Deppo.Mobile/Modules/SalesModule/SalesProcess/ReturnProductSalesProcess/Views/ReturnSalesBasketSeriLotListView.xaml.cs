using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesProcess.Views;

public partial class ReturnSalesBasketSeriLotListView : ContentPage
{
    private readonly ReturnSalesBasketSeriLotListViewModel _viewModel;

    public ReturnSalesBasketSeriLotListView(ReturnSalesBasketSeriLotListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
    }
}