using Deppo.Mobile.Modules.ProductModule.ProductProcess.VirmanProductProcess.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.VirmanProductProcess.Views;

public partial class VirmanProductOutWarehouseListView : ContentPage
{
    private readonly VirmanProductOutWarehouseListViewModel _viewModel;

    public VirmanProductOutWarehouseListView(VirmanProductOutWarehouseListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}