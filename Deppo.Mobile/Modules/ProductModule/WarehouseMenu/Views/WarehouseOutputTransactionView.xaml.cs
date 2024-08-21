using Deppo.Mobile.Modules.ProductModule.WarehouseMenu.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.WarehouseMenu.Views;

public partial class WarehouseOutputTransactionView : ContentPage
{
	WarehouseOutputTransactionViewModel _viewModel;
	public WarehouseOutputTransactionView(WarehouseOutputTransactionViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}
}