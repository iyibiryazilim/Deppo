using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesDispatchProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesDispatchProcess.Views;

public partial class ReturnSalesDispatchBasketView : ContentPage
{
    private readonly ReturnSalesDispatchBasketViewModel _viewModel;

    public ReturnSalesDispatchBasketView(ReturnSalesDispatchBasketViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}