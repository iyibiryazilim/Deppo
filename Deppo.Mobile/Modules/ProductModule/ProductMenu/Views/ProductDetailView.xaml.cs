using Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.Views;

public partial class ProductDetailView : ContentPage
{
    private ProductDetailViewModel _viewModel;

    public ProductDetailView(ProductDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
        _viewModel.CurrentPage = this;
    }
}