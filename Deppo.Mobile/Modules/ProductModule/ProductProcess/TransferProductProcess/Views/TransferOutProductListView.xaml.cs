using Deppo.Mobile.Modules.ProductModule.ProductProcess.TransferProductProcess.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.TransferProductProcess.Views;

public partial class TransferOutProductListView : ContentPage
{
	private readonly TransferOutProductListViewModel _viewModel;
    public TransferOutProductListView(TransferOutProductListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.SearchText = searchBar;
        _viewModel.CurrentPage = this;
    }
}