using Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels.ActionViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.Views.ActionViews;

public partial class ProductDetailAlternativeProductListView : ContentPage
{
	private readonly ProductDetailAlternativeProductListViewModel _viewModel;
	public ProductDetailAlternativeProductListView(ProductDetailAlternativeProductListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}