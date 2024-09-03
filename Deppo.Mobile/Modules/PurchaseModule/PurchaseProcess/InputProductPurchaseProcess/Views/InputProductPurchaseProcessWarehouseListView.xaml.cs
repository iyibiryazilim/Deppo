using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.Views;

public partial class InputProductPurchaseProcessWarehouseListView : ContentPage
{
    private readonly InputProductPurchaseProcessWarehouseListViewModel _viewModel;

    public InputProductPurchaseProcessWarehouseListView(InputProductPurchaseProcessWarehouseListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}