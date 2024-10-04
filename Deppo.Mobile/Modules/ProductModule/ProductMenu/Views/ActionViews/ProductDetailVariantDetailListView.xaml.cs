using Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels.ActionViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.Views.ActionViews;

public partial class ProductDetailVariantDetailListView : ContentPage
{
	private readonly ProductDetailVariantDetailListViewModel _viewModel;
    public ProductDetailVariantDetailListView(ProductDetailVariantDetailListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}