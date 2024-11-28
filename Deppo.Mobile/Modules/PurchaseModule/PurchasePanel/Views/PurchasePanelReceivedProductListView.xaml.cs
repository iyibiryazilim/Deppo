using Deppo.Mobile.Modules.PurchaseModule.PurchasePanel.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchasePanel.Views;

public partial class PurchasePanelReceivedProductListView : ContentPage
{
	private readonly PurchasePanelReceivedProductListViewModel _viewModel;
    public PurchasePanelReceivedProductListView(PurchasePanelReceivedProductListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}