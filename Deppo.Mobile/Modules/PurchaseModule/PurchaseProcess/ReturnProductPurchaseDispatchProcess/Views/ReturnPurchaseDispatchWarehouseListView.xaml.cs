using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseDispatchProcess.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseDispatchProcess.Views;

public partial class ReturnPurchaseDispatchWarehouseListView : ContentPage
{
	private readonly ReturnPurchaseDispatchWarehouseListViewModel _viewModel;
    public ReturnPurchaseDispatchWarehouseListView(ReturnPurchaseDispatchWarehouseListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}