using Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels.ActionViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.Views.ActionViews;

public partial class ProductDetailApprovedSupplierView : ContentPage
{
	private readonly ProductDetailApprovedSupplierViewModel _viewModel;
    public ProductDetailApprovedSupplierView(ProductDetailApprovedSupplierViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

}