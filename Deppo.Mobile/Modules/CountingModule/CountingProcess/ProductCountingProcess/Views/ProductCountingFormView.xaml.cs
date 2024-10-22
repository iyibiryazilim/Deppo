using Deppo.Mobile.Modules.CountingModule.CountingProcess.ProductCountingProcess.ViewModels;

namespace Deppo.Mobile.Modules.CountingModule.CountingProcess.ProductCountingProcess.Views;

public partial class ProductCountingFormView : ContentPage
{
	private readonly ProductCountingFormViewModel _viewModel;
	public ProductCountingFormView(ProductCountingFormViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
		_viewModel.CurrentPage = this;
	}
}