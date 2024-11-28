using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.Views;

public partial class PurchaseProcessView : ContentPage
{
    private readonly PurchaseProcessViewModel _viewModel;

    public PurchaseProcessView(PurchaseProcessViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}