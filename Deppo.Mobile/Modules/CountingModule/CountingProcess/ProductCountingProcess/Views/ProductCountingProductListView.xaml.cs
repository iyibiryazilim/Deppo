using Deppo.Mobile.Modules.CountingModule.CountingProcess.ProductCountingProcess.ViewModels;

namespace Deppo.Mobile.Modules.CountingModule.CountingProcess.ProductCountingProcess.Views;

public partial class ProductCountingProductListView : ContentPage
{
	private readonly ProductCountingProductListViewModel _viewModel;
	public ProductCountingProductListView(ProductCountingProductListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.SearchText = searchBar;
		_viewModel.CurrentPage = this;
	}
}