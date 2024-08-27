using Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.Views;

public partial class OutputProductProcessWarehouseListView : ContentPage
{
	private readonly OutputProductProcessWarehouseListViewModel _viewModel;
	public OutputProductProcessWarehouseListView(OutputProductProcessWarehouseListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}