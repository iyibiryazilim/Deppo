using Deppo.Mobile.Modules.SalesModule.CustomerMenu.ViewModels.ActionViewModels;
using DevExpress.Data.Async.Helpers;

namespace Deppo.Mobile.Modules.SalesModule.CustomerMenu.Views.ActionViews;

public partial class CustomerDetailWaitingSalesOrderListView : ContentPage
{
	private readonly CustomerDetailWaitingSalesOrderListViewModel _viewModel;
	public CustomerDetailWaitingSalesOrderListView(CustomerDetailWaitingSalesOrderListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}