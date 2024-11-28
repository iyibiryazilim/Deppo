using Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.Views;

public partial class SupplierOutputTransactionView : ContentPage
{
    private readonly SupplierOutputTransactionViewModel _viewModel;

    public SupplierOutputTransactionView(SupplierOutputTransactionViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.SearchText = searchBar;
    }
}