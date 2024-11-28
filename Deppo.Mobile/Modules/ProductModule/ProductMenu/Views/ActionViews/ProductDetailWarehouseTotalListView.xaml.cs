using Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels.ActionViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.Views.ActionViews;

public partial class ProductDetailWarehouseTotalListView : ContentPage
{
	private readonly ProductDetailWarehouseTotalListViewModel _viewModel;
    public ProductDetailWarehouseTotalListView(ProductDetailWarehouseTotalListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}