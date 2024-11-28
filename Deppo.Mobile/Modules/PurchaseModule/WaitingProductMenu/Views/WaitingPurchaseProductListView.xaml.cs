using Deppo.Mobile.Modules.PurchaseModule.WaitingProductMenu.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.WaitingProductMenu.Views;

public partial class WaitingPurchaseProductListView : ContentPage
{
	private readonly WaitingPurchaseProductListViewModel _viewModel;
	public WaitingPurchaseProductListView(WaitingPurchaseProductListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
		_viewModel.CurrentPage = this;
	}
}