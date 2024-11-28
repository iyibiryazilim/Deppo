using AndroidX.Lifecycle;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.Views;

public partial class InputProductPurchaseOrderProcessWarehouseListView : ContentPage
{
    private readonly InputProductPurchaseOrderProcessWarehouseListViewModel _viewModel;

    public InputProductPurchaseOrderProcessWarehouseListView(InputProductPurchaseOrderProcessWarehouseListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}