using Deppo.Mobile.Modules.ProductModule.ProductProcess.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.Views;

public partial class ProductProcessView : ContentPage
{
	private readonly ProductProcessViewModel _viewModel;
	public ProductProcessView(ProductProcessViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}