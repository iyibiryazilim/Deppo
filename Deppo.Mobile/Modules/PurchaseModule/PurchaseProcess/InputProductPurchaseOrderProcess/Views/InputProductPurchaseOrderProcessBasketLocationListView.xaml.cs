using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.Views;

public partial class InputProductPurchaseOrderProcessBasketLocationListView : ContentPage
{
    private readonly InputProductPurchaseOrderProcessBasketLocationListViewModel _viewModel;

    public InputProductPurchaseOrderProcessBasketLocationListView(InputProductPurchaseOrderProcessBasketLocationListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
        _viewModel.SearchText = locationSearchBar;
	}
}