using Deppo.Mobile.Modules.ProductModule.WarehouseMenu.ViewModels.ActionViewModels;

namespace Deppo.Mobile.Modules.ProductModule.WarehouseMenu.Views.ActionViews;

public partial class WarehouseDetailWarehouseTotalListView : ContentPage
{
    private readonly WarehouseDetailWarehouseTotalListViewModel _viewModel;

    public WarehouseDetailWarehouseTotalListView(WarehouseDetailWarehouseTotalListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}