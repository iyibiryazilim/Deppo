using Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.Views;

public partial class ProductOutputTransactionView : ContentPage
{
	ProductOutputTransactionViewModel _viewModel;
	public ProductOutputTransactionView(ProductOutputTransactionViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}
}