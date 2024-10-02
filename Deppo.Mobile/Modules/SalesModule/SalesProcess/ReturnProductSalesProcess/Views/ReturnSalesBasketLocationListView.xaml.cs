using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesProcess.Views;

public partial class ReturnSalesBasketLocationListView : ContentPage
{
    private readonly ReturnSalesBasketLocationListViewModel _viewModel;

    public ReturnSalesBasketLocationListView(ReturnSalesBasketLocationListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
        _viewModel.SearchText = locationSearchBar;
    }
}