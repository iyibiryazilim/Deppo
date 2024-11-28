using Deppo.Mobile.Modules.PurchaseModule.WaitingOrderMenu.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.WaitingOrderMenu.Views;

public partial class WaitingPurchaseOrderListView : ContentPage
{
    private readonly WaitingPurchaseOrderListViewModel _viewModel;

    public WaitingPurchaseOrderListView(WaitingPurchaseOrderListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}