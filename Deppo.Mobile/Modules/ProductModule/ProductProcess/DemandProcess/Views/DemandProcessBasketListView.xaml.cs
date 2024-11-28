using Deppo.Mobile.Modules.ProductModule.ProductProcess.DemandProcess.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.DemandProcess.Views;

public partial class DemandProcessBasketListView : ContentPage
{
	private readonly DemandProcessBasketListViewModel _viewModel;
    public DemandProcessBasketListView(DemandProcessBasketListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
		_viewModel.BarcodeEntry = barcodeEntry;
		_viewModel.BarcodeEntry.Focus();
	}
}