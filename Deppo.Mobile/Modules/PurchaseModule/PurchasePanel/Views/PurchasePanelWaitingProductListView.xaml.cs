using Deppo.Mobile.Modules.PurchaseModule.PurchasePanel.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchasePanel.Views;

public partial class PurchasePanelWaitingProductListView : ContentPage
{
    private readonly PurchasePanelWaitingProductListViewModel _viewModel;
    public PurchasePanelWaitingProductListView(PurchasePanelWaitingProductListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}