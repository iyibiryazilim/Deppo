using Deppo.Mobile.Modules.ProductModule.ProductPanel.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductPanel.Views;

public partial class ProductPanelView : ContentPage
{
	private readonly ProductPanelViewModel _viewModel;
	public ProductPanelView(ProductPanelViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}