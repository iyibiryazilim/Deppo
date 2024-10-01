using Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.Views;

public partial class OutputProductProcessProductListView : ContentPage
{
	private readonly OutputProductProcessProductListViewModel _viewModel;
	public OutputProductProcessProductListView(OutputProductProcessProductListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
		_viewModel.SearchText = searchBar;
	}
}