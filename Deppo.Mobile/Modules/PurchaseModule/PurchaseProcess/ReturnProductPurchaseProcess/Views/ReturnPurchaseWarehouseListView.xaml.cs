using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseProcess.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseProcess.Views;

public partial class ReturnPurchaseWarehouseListView : ContentPage
{
	private readonly ReturnPurchaseWarehouseListViewModel _viewModel;
    public ReturnPurchaseWarehouseListView(ReturnPurchaseWarehouseListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}