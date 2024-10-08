using Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.Views;

public partial class SupplierInputTransactionView : ContentPage
{
    private readonly SupplierInputTransactionViewModel _viewModel;

    public SupplierInputTransactionView(SupplierInputTransactionViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.SearchText = searchBar;
    }
}