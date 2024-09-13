using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.Views;

public partial class InputProductPurchaseOrderProcessFormView : ContentPage
{
    private readonly InputProductPurchaseOrderProcessFormViewModel _viewModel;

    public InputProductPurchaseOrderProcessFormView(InputProductPurchaseOrderProcessFormViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}