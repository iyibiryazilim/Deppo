using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.Views;

public partial class ProcurementSalesProcessProductBasketView : ContentPage
{
	private readonly ProcurementSalesProcessProductBasketViewModel _viewModel;
    public ProcurementSalesProcessProductBasketView(ProcurementSalesProcessProductBasketViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.BarcodeEntry = barcodeEntry;
        _viewModel.BarcodeEntry.Focus();
        _viewModel.CurrentPage = this;
    }
}