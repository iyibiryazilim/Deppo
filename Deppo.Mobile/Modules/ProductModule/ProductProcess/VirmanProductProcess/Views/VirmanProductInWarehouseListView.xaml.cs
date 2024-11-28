using Deppo.Mobile.Modules.ProductModule.ProductProcess.VirmanProductProcess.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.VirmanProductProcess.Views;

public partial class VirmanProductInWarehouseListView : ContentPage
{
    private readonly VirmanProductInWarehouseListViewModel _viewModel;

    public VirmanProductInWarehouseListView(VirmanProductInWarehouseListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
        _viewModel.CurrentPage = this;
    }
}