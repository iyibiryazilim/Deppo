using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseDispatchProcess.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseDispatchProcess.Views;

public partial class ReturnPurchaseDispatchProductListView : ContentPage
{
    private readonly ReturnPurchaseDispatchProductListViewModel _viewModel;
    public ReturnPurchaseDispatchProductListView(ReturnPurchaseDispatchProductListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
        _viewModel.SearchText = searchBar;
    }
}