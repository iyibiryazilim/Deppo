using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseProcess.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseProcess.Views;

public partial class ReturnPurchaseFormView : ContentPage
{
	private readonly ReturnPurchaseFormViewModel _viewModel;
    public ReturnPurchaseFormView(ReturnPurchaseFormViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}