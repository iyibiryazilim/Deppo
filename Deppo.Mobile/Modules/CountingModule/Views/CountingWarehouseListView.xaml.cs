using Deppo.Mobile.Modules.CountingModule.ViewModels;

namespace Deppo.Mobile.Modules.CountingModule.Views;

public partial class CountingWarehouseListView : ContentPage
{
	private readonly CountingWarehouseListViewModel _viewModel;
	public CountingWarehouseListView(CountingWarehouseListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}