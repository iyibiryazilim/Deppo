using Deppo.Mobile.Modules.ProductModule.ProductPanel.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductPanel.Views;

public partial class InputProductListView : ContentPage
{
	private readonly InputProductListViewModel _viewModel;
	public InputProductListView(InputProductListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}