using Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels.ActionViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.Views.ActionViews;

public partial class ProductDetailVariantTotalListView : ContentPage
{
	private readonly ProductDetailVariantTotalListViewModel _viewModel;
    public ProductDetailVariantTotalListView(ProductDetailVariantTotalListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}