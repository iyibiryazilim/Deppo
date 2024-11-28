using Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels.ActionViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.Views.ActionViews;

public partial class ProductDetailVariantListView : ContentPage
{
	private readonly ProductDetailVariantListViewModel _viewModel;
    public ProductDetailVariantListView(ProductDetailVariantListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}