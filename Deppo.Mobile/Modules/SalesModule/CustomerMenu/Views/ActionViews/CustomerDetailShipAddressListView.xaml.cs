using Deppo.Mobile.Modules.SalesModule.CustomerMenu.ViewModels.ActionViewModels;

namespace Deppo.Mobile.Modules.SalesModule.CustomerMenu.Views.ActionViews;

public partial class CustomerDetailShipAddressListView : ContentPage
{
	private readonly CustomerDetailShipAddressListViewModel _viewModel;

	public CustomerDetailShipAddressListView(CustomerDetailShipAddressListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}