using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.Views;

public partial class OutputProductSalesProcessProductListView : ContentPage
{
	private readonly OutputProductSalesProcessProductListViewModel _viewModel;
	public OutputProductSalesProcessProductListView(OutputProductSalesProcessProductListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
		_viewModel.SearchText = searchBar;
    }
}