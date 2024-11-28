using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;

public partial class InputProductProcessBasketLocationListView : ContentPage
{
	private readonly InputProductProcessBasketLocationListViewModel _viewModel;
	public InputProductProcessBasketLocationListView(InputProductProcessBasketLocationListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
		_viewModel.SearchText = locationSearchBar;
	}
}