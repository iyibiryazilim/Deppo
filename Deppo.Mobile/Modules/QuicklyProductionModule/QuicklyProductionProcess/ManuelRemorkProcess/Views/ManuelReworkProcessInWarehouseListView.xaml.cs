using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.ViewModels;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.Views;

public partial class ManuelReworkProcessInWarehouseListView : ContentPage
{
	private readonly ManuelReworkProcessInWarehouseListViewModel _viewModel;
	public ManuelReworkProcessInWarehouseListView(ManuelReworkProcessInWarehouseListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}