using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseProcess.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseProcess.Views;

public partial class ReturnPurchaseProductListView : ContentPage
{
	private readonly ReturnPurchaseProductListViewModel _viewModel;
    public ReturnPurchaseProductListView(ReturnPurchaseProductListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
        _viewModel.SearchText = searchBar;
    }
}