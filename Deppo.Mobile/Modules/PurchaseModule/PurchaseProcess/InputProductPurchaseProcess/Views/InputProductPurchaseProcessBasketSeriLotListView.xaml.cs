using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.Views;

public partial class InputProductPurchaseProcessBasketSeriLotListView : ContentPage
{
    private readonly InputProductPurchaseProcessBasketSeriLotListViewModel _viewModel;

    public InputProductPurchaseProcessBasketSeriLotListView(InputProductPurchaseProcessBasketSeriLotListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}