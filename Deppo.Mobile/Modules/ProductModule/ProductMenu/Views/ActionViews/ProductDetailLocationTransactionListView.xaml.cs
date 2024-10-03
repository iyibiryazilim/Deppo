using Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels.ActionViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.Views.ActionViews;

public partial class ProductDetailLocationTransactionListView : ContentPage
{
	private readonly ProductDetailLocationTransactionListViewModel _viewModel;
    public ProductDetailLocationTransactionListView(ProductDetailLocationTransactionListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}