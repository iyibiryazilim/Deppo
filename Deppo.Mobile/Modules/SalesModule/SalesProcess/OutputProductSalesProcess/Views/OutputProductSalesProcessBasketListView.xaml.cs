using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.Views;

public partial class OutputProductSalesProcessBasketListView : ContentPage
{
	private readonly OutputProductSalesProcessBasketListViewModel _viewModel;
	public OutputProductSalesProcessBasketListView(OutputProductSalesProcessBasketListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
		_viewModel.BarcodeEntry = barcodeEntry;
		_viewModel.BarcodeEntry.Focus();
	}
}