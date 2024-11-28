using Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.Views;

public partial class ProductInputTransactionView : ContentPage
{
	ProductInputTransactionViewModel _viewModel;
	public ProductInputTransactionView(ProductInputTransactionViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}
}