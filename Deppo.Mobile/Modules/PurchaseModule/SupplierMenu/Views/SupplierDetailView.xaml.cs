using Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.Views;

public partial class SupplierDetailView : ContentPage
{
    private readonly SupplierDetailViewModel _viewModel;

    public SupplierDetailView(SupplierDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}