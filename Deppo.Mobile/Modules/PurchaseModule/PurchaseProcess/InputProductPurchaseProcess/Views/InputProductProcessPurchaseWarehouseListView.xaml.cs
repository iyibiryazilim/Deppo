using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.Views;

public partial class InputProductProcessPurchaseWarehouseListView : ContentPage
{
    private readonly InputProductProcessPurchaseWarehouseListViewModel _viewModel;

    public InputProductProcessPurchaseWarehouseListView(InputProductProcessPurchaseWarehouseListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}