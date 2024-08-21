using Deppo.Mobile.Modules.ProductModule.WarehouseMenu.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.WarehouseMenu.Views;

public partial class WarehouseInputTransactionView : ContentPage
{
	WarehouseInputTransactionViewModel _viewModel;
	public WarehouseInputTransactionView(WarehouseInputTransactionViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}
}