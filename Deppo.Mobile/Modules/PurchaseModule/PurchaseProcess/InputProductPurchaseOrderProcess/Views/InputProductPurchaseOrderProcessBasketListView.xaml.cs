using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.Views;

public partial class InputProductPurchaseOrderProcessBasketListView : ContentPage
{
    private readonly InputProductPurchaseOrderProcessBasketListViewModel _viewModel;

    public InputProductPurchaseOrderProcessBasketListView(InputProductPurchaseOrderProcessBasketListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
		_viewModel.BarcodeEntry = barcodeEntry;
		_viewModel.BarcodeEntry.Focus();
	}
}