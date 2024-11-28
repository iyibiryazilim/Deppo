using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;

public partial class InputProductProcessWarehouseListView : ContentPage
{
	private readonly InputProductProcessWarehouseListViewModel _viewModel;
	public InputProductProcessWarehouseListView(InputProductProcessWarehouseListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}