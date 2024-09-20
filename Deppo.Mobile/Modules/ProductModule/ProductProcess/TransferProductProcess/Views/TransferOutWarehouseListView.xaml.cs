using Deppo.Mobile.Modules.ProductModule.ProductProcess.TransferProductProcess.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.TransferProductProcess.Views;

public partial class TransferOutWarehouseListView : ContentPage
{
	private readonly TransferOutWarehouseListViewModel _viewModel;
    public TransferOutWarehouseListView(TransferOutWarehouseListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}