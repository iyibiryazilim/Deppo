using Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.Views;

public partial class SupplierListView : ContentPage
{
	private readonly SupplierListViewModel _viewModel;
	public SupplierListView(SupplierListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.SearchText = searchBar;
    }
}