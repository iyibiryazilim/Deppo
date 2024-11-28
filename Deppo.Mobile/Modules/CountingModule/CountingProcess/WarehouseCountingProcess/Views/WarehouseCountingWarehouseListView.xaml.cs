using Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.ViewModels;

namespace Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.Views;

public partial class WarehouseCountingWarehouseListView : ContentPage
{
	private readonly WarehouseCountingWarehouseListViewModel _viewModel;
	public WarehouseCountingWarehouseListView(WarehouseCountingWarehouseListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
    }
}