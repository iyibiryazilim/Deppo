using CommunityToolkit.Maui.Core.Platform;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.Views;

public partial class InputProductPurchaseProcessBasketListView : ContentPage
{
    private readonly InputProductPurchaseProcessBasketListViewModel _viewModel;

    public InputProductPurchaseProcessBasketListView(InputProductPurchaseProcessBasketListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
		_viewModel.BarcodeEntry = barcodeEntry;
        barcodeEntry.Focus();
        barcodeEntry.HideKeyboardAsync();
	}
}