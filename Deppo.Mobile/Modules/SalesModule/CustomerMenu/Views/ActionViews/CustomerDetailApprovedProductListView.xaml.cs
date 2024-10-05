using Deppo.Mobile.Modules.SalesModule.CustomerMenu.ViewModels.ActionViewModels;

namespace Deppo.Mobile.Modules.SalesModule.CustomerMenu.Views.ActionViews;

public partial class CustomerDetailApprovedProductListView : ContentPage
{
	private readonly CustomerDetailApprovedProductListViewModel _viewModel;
	public CustomerDetailApprovedProductListView(CustomerDetailApprovedProductListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}
}