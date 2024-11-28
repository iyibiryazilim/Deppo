using Deppo.Mobile.Modules.ProductModule.ProductPanel.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductPanel.Views;

public partial class OutputProductListView : ContentPage
{
	private readonly OutputProductListViewModel _viewModel;
	public OutputProductListView(OutputProductListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}