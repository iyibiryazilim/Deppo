using Deppo.Mobile.Modules.ProductModule.ProductProcess.ProductionInput.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.ProductionInput.Views;

public partial class ProductionInputWarehouseListView : ContentPage
{
	private ProductionInputWarehouseListViewModel _viewModel;
	public ProductionInputWarehouseListView(ProductionInputWarehouseListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}