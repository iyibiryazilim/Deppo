using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.ViewModels;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.Views;

public partial class ManuelCalcWarehouseListView : ContentPage
{
    private readonly ManuelCalcWarehouseListViewModel _viewModel;

    public ManuelCalcWarehouseListView(ManuelCalcWarehouseListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}