using AndroidX.Lifecycle;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.Views;

public partial class InputProductPurchaseProcessFormView : ContentPage
{
    private readonly InputProductPurchaseProcessFormViewModel _viewModel;

    public InputProductPurchaseProcessFormView(InputProductPurchaseProcessFormViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}