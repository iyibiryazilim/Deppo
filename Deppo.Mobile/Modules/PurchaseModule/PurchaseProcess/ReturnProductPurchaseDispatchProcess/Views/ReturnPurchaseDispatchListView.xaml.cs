using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseDispatchProcess.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseDispatchProcess.Views;

public partial class ReturnPurchaseDispatchListView : ContentPage
{
	private readonly ReturnPurchaseDispatchListViewModel _viewModel;
    public ReturnPurchaseDispatchListView(ReturnPurchaseDispatchListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.SearchText = searchBar;
    }
   
}