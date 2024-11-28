using Deppo.Mobile.Modules.CountingModule.CountingProcess.ProductCountingProcess.ViewModels;

namespace Deppo.Mobile.Modules.CountingModule.CountingProcess.ProductCountingProcess.Views;

public partial class ProductCountingWarehouseTotalListView : ContentPage
{
	private readonly ProductCountingWarehouseTotalListViewModel _viewModel;
	public ProductCountingWarehouseTotalListView(ProductCountingWarehouseTotalListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}