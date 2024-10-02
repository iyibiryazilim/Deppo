using Deppo.Mobile.Modules.CountingModule.CountingProcess.ProductCountingProcess.ViewModels;

namespace Deppo.Mobile.Modules.CountingModule.CountingProcess.ProductCountingProcess.Views;

public partial class ProductCountingLocationListView : ContentPage
{
	private readonly ProductCountingLocationListViewModel _viewModel;
	public ProductCountingLocationListView(ProductCountingLocationListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.SearchText = searchBar;
    }
}