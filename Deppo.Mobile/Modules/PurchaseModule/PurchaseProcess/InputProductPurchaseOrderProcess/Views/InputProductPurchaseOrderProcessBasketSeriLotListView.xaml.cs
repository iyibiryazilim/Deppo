using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.Views;

public partial class InputProductPurchaseOrderProcessBasketSeriLotListView : ContentPage
{
    private readonly InputProductPurchaseOrderProcessBasketSeriLotListViewModel _viewModel;

    public InputProductPurchaseOrderProcessBasketSeriLotListView(InputProductPurchaseOrderProcessBasketSeriLotListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}