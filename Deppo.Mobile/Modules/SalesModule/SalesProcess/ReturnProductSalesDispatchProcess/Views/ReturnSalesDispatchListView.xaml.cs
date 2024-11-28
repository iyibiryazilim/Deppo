using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesDispatchProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesDispatchProcess.Views;

public partial class ReturnSalesDispatchListView : ContentPage
{
    private readonly ReturnSalesDispatchListViewModel _viewModel;

    public ReturnSalesDispatchListView(ReturnSalesDispatchListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.SearchText = searchBar;
    }
}