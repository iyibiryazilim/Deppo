using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesProcess.Views;

public partial class ReturnSalesBasketView : ContentPage
{
    private readonly ReturnSalesBasketViewModel _viewModel;

    public ReturnSalesBasketView(ReturnSalesBasketViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}