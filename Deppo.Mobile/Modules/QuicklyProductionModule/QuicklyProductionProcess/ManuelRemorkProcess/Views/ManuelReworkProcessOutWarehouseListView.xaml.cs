using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.ViewModels;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.Views;

public partial class ManuelReworkProcessOutWarehouseListView : ContentPage
{
	private readonly ManuelReworkProcessOutWarehouseListViewModel _viewModel;
	public ManuelReworkProcessOutWarehouseListView(ManuelReworkProcessOutWarehouseListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}
}