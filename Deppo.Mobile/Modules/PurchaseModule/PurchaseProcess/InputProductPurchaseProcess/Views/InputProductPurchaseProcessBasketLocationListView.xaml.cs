using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.Views;

public partial class InputProductPurchaseProcessBasketLocationListView : ContentPage
{
    private readonly InputProductPurchaseProcessBasketLocationListViewModel _viewModel;

    public InputProductPurchaseProcessBasketLocationListView(InputProductPurchaseProcessBasketLocationListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
        _viewModel.SearchText = locationSearchBar;

	}
}