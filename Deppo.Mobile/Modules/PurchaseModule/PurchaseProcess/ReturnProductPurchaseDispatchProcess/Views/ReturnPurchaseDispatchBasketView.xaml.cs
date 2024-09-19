using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseDispatchProcess.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseDispatchProcess.Views;

public partial class ReturnPurchaseDispatchBasketView : ContentPage
{
	private readonly ReturnPurchaseDispatchBasketViewModel _viewModel;
    public ReturnPurchaseDispatchBasketView(ReturnPurchaseDispatchBasketViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}