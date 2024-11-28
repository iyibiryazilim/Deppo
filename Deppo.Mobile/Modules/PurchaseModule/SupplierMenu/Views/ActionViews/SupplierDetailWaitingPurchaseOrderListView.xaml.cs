using Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.ViewModels.ActionViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.Views.ActionViews;

public partial class SupplierDetailWaitingPurchaseOrderListView : ContentPage
{
	private readonly SupplierDetailWaitingPurchaseOrderListViewModel _viewModel;
	public SupplierDetailWaitingPurchaseOrderListView(SupplierDetailWaitingPurchaseOrderListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}