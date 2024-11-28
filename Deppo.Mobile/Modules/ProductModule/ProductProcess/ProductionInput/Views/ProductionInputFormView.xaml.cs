using Deppo.Mobile.Modules.ProductModule.ProductProcess.ProductionInput.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.ProductionInput.Views;

public partial class ProductionInputFormView : ContentPage
{
	private readonly ProductionInputFormViewModel _viewModel;
	public ProductionInputFormView(ProductionInputFormViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}