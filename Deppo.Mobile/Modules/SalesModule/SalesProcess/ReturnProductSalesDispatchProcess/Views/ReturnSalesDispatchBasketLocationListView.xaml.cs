using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesDispatchProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesDispatchProcess.Views;

public partial class ReturnSalesDispatchBasketLocationListView : ContentPage
{
    private readonly ReturnSalesDispatchBasketLocationListViewModel _viewModel;

    public ReturnSalesDispatchBasketLocationListView(ReturnSalesDispatchBasketLocationListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
        _viewModel.SearchText = locationSearchBar;

	}
}