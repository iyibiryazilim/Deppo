using Deppo.Mobile.Modules.CountingModule.CountingProcess.ProductCountingProcess.ViewModels;

namespace Deppo.Mobile.Modules.CountingModule.CountingProcess.ProductCountingProcess.Views;

public partial class ProductCountingBasketView : ContentPage
{
	private readonly ProductCountingBasketViewModel _viewModel;
	public ProductCountingBasketView(ProductCountingBasketViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}