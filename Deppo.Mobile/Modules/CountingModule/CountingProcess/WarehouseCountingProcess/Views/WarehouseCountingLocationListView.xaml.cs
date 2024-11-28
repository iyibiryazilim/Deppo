using Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.ViewModels;

namespace Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.Views;

public partial class WarehouseCountingLocationListView : ContentPage
{
	private readonly WarehouseCountingLocationListViewModel _viewModel;
	public WarehouseCountingLocationListView(WarehouseCountingLocationListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.SearchText = searchBar;
		_viewModel.CurrentPage = this;
    }
}