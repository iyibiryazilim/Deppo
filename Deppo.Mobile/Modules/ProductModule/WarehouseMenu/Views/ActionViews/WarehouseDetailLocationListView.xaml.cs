using Deppo.Mobile.Modules.ProductModule.WarehouseMenu.ViewModels.ActionViewModels;

namespace Deppo.Mobile.Modules.ProductModule.WarehouseMenu.Views.ActionViews;

public partial class WarehouseDetailLocationListView : ContentPage
{
    private readonly WarehouseDetailLocationListViewModel _viewModel;

    public WarehouseDetailLocationListView(WarehouseDetailLocationListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}