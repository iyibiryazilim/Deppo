using Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.Views;

public partial class ProductListView : ContentPage
{
	private readonly ProductListViewModel _viewModel;
	public ProductListView(ProductListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.SearchText = searchBar;
	}

}