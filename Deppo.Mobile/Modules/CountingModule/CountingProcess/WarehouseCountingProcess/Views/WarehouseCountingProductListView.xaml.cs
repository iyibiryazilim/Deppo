using Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.ViewModels;

namespace Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.Views;

public partial class WarehouseCountingProductListView : ContentPage
{
	private readonly WarehouseCountingProductListViewModel _viewModel;
	public WarehouseCountingProductListView(WarehouseCountingProductListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}