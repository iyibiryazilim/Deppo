using Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.ViewModels.ActionViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.Views.ActionViews;

public partial class SupplierDetailShipAddressListView : ContentPage
{
	private readonly SupplierDetailShipAddressListViewModel _viewModel;
	public SupplierDetailShipAddressListView(SupplierDetailShipAddressListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}