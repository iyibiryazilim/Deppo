using Deppo.Mobile.Modules.ProductModule.ProductProcess.TransferProductProcess.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.TransferProductProcess.Views;

public partial class TransferOutBasketView : ContentPage
{

	private readonly TransferOutBasketViewModel _viewModel;
    public TransferOutBasketView(TransferOutBasketViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
		_viewModel.BarcodeEntry = barcodeEntry;
		_viewModel.BarcodeEntry.Focus();
	}

}