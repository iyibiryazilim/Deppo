using Deppo.Mobile.Modules.ProductModule.ProductProcess.ProductionInput.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.ProductionInput.Views;

public partial class ProductionInputProductListView : ContentPage
{
	private ProductionInputProductListViewModel _viewModel;
	public ProductionInputProductListView(ProductionInputProductListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}