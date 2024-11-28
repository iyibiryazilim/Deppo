using Deppo.Mobile.Modules.ProductModule.ProductProcess.ProductionInput.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.ProductionInput.Views;

public partial class ProductionInputResultView : ContentPage
{
	private readonly ProductionInputResultViewModel _viewModel;
	public ProductionInputResultView(ProductionInputResultViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}