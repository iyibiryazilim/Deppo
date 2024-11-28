using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;

public partial class InputProductProcessProductListView : ContentPage
{
	private readonly InputProductProcessProductListViewModel _viewModel;
	public InputProductProcessProductListView(InputProductProcessProductListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
		_viewModel.SearchText = searchBar;
	}
}