using Deppo.Mobile.Modules.ProductModule.WarehouseMenu.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.WarehouseMenu.Views;

public partial class WarehouseDetailAllFicheListView : ContentPage
{
    private readonly WarehouseDetailAllFicheListViewModel _viewModel;

    public WarehouseDetailAllFicheListView(WarehouseDetailAllFicheListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}