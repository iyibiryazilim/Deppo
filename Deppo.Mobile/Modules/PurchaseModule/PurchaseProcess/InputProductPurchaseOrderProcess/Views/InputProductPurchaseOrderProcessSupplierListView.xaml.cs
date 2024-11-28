using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.Views;

public partial class InputProductPurchaseOrderProcessSupplierListView : ContentPage
{
    private readonly InputProductPurchaseOrderProcessSupplierListViewModel _viewModel;

    public InputProductPurchaseOrderProcessSupplierListView(InputProductPurchaseOrderProcessSupplierListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
        _viewModel.SearchText = searchBar;
    }
}