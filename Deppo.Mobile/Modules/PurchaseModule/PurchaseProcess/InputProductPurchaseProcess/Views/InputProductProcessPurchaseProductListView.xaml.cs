using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.Views;

public partial class InputProductProcessPurchaseProductListView : ContentPage
{
    private readonly InputProductProcessPurchaseProductListViewModel _viewModel;

    public InputProductProcessPurchaseProductListView(InputProductProcessPurchaseProductListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}