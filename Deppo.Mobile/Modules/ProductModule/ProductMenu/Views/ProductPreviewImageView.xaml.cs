using Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.Views;

public partial class ProductPreviewImageView : ContentPage
{
	private readonly ProductPreviewImageViewModel _viewModel;
	public ProductPreviewImageView(ProductPreviewImageViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}