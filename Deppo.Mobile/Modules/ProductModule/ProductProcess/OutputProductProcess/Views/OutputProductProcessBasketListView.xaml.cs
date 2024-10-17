using Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.Views;

public partial class OutputProductProcessBasketListView : ContentPage
{
	private readonly OutputProductProcessBasketListViewModel _viewModel;
	public OutputProductProcessBasketListView(OutputProductProcessBasketListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
		_viewModel.LocationTransactionSearchText = locationTransactionSearchBar;
		_viewModel.BarcodeEntry = barcodeEntry;
		_viewModel.BarcodeEntry.Focus();

	}
}