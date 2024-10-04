using Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels.ActionViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.Views.ActionViews;

public partial class ProductDetailWaitingPurchaseOrderListView : ContentPage
{
	private readonly ProductDetailWaitingPurchaseOrderListViewModel _viewModel;
    public ProductDetailWaitingPurchaseOrderListView(ProductDetailWaitingPurchaseOrderListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}