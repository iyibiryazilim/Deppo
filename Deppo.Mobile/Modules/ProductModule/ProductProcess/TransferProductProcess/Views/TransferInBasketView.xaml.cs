using Deppo.Mobile.Modules.ProductModule.ProductProcess.TransferProductProcess.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.TransferProductProcess.Views;

public partial class TransferInBasketView : ContentPage
{
	private readonly TransferInBasketViewModel _viewModel;
    public TransferInBasketView(TransferInBasketViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
        _viewModel.LocationSearchText = locationSearchBar;
    }
}