using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseDispatchProcess.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseDispatchProcess.Views;

public partial class ReturnPurchaseDispatchFormView : ContentPage
{
	private readonly ReturnPurchaseDispatchFormViewModel _viewModel;
    public ReturnPurchaseDispatchFormView(ReturnPurchaseDispatchFormViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}