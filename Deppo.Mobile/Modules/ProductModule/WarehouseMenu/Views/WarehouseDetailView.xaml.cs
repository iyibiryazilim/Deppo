using Deppo.Mobile.Modules.ProductModule.WarehouseMenu.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.WarehouseMenu.Views;

public partial class WarehouseDetailView : ContentPage
{
    private WarehouseDetailViewModel _viewModel;

    public WarehouseDetailView(WarehouseDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
        _viewModel.CurrentPage = this;
    }
}