using Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels.ActionViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.Views.ActionViews;

public partial class ProductDetailWaitingSalesOrderListView : ContentPage
{
	private readonly ProductDetailWaitingSalesOrderListViewModel _viewModel;
    public ProductDetailWaitingSalesOrderListView(ProductDetailWaitingSalesOrderListViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}