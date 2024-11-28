using Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.ViewModels.ActionViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.Views.ActionViews;

public partial class SupplierDetailApprovedProductListView : ContentPage
{
	private readonly SupplierDetailApprovedProductListViewModel _viewModel;
	public SupplierDetailApprovedProductListView(SupplierDetailApprovedProductListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}