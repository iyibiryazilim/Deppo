using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.Views;

public partial class InputProductPurchaseProcessSupplierListView : ContentPage
{
    private readonly InputProductPurchaseProcessSupplierListViewModel _viewModel;

    public InputProductPurchaseProcessSupplierListView(InputProductPurchaseProcessSupplierListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.SearchText = searchBar;
        _viewModel.CurrentPage = this;
    }
}