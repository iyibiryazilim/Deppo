using Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.ViewModels;

namespace Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.Views;

public partial class WarehouseCountingShowProductListView : ContentPage
{
	private readonly WarehouseCountingShowProductListViewModel _viewModel;
	public WarehouseCountingShowProductListView(WarehouseCountingShowProductListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.SearchText = searchBar;
	}
}