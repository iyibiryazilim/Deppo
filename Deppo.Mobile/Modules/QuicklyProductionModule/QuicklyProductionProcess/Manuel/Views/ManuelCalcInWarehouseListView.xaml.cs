using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.ViewModels;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.Views;

public partial class ManuelCalcInWarehouseListView : ContentPage
{
    private readonly ManuelCalcInWarehouseListViewModel _viewModel;

    public ManuelCalcInWarehouseListView(ManuelCalcInWarehouseListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}