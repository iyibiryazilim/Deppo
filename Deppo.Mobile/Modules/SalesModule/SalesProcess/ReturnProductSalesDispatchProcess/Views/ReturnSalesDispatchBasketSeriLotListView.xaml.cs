using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesDispatchProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesDispatchProcess.Views;

public partial class ReturnSalesDispatchBasketSeriLotListView : ContentPage
{
    private readonly ReturnSalesDispatchBasketSeriLotListViewModel _viewModel;

    public ReturnSalesDispatchBasketSeriLotListView(ReturnSalesDispatchBasketSeriLotListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}