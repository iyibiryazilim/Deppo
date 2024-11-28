using Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.Views;

public partial class ProductPictureView : ContentPage
{
	private readonly ProductPictureViewModel _viewModel;
	public ProductPictureView(ProductPictureViewModel viewModel)
	{
		InitializeComponent();
		 _viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;

	}
}