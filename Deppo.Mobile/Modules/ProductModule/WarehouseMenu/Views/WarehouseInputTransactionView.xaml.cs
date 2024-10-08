using Deppo.Mobile.Modules.ProductModule.WarehouseMenu.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.WarehouseMenu.Views;

public partial class WarehouseInputTransactionView : ContentPage
{
    private WarehouseInputTransactionViewModel _viewModel;

    public WarehouseInputTransactionView(WarehouseInputTransactionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
        _viewModel.CurrentPage = this;
    }
}