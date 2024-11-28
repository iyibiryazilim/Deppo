using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;

public partial class InputProductProcessBasketSeriLotListView : ContentPage
{
	private readonly InputProductProcessBasketSeriLotListViewModel _viewModel;
	public InputProductProcessBasketSeriLotListView(InputProductProcessBasketSeriLotListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}