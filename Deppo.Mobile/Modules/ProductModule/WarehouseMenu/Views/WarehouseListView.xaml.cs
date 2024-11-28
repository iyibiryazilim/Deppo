using Deppo.Mobile.Modules.ProductModule.WarehouseMenu.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.WarehouseMenu.Views;

public partial class WarehouseListView : ContentPage
{
	private readonly WarehouseListViewModel _viewModel;
	public WarehouseListView(WarehouseListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.SearchText = searchBar;
	}
}