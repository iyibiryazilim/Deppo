using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.Views;

public partial class InputProductPurchaseOrderProcessOtherProductListView : ContentPage
{
	private readonly InputProductPurchaseOrderProcessOtherProductListViewModel _viewModel;
    public InputProductPurchaseOrderProcessOtherProductListView(InputProductPurchaseOrderProcessOtherProductListViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}