using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.Views;

public partial class InputProductProcessPurchaseSupplierListView : ContentPage
{
    private readonly InputProductProcessPurchaseSupplierListViewModel _viewModel;

    public InputProductProcessPurchaseSupplierListView(InputProductProcessPurchaseSupplierListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}