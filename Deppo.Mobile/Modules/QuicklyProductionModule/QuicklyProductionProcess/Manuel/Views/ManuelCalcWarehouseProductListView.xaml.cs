using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.ViewModels;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.Views;

public partial class ManuelCalcWarehouseProductListView : ContentPage
{
    private readonly ManuelCalcWarehouseProductListViewModel _viewModel;

    public ManuelCalcWarehouseProductListView(ManuelCalcWarehouseProductListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}