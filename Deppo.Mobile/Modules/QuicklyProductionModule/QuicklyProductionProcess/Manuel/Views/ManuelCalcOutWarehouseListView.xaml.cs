using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.ViewModels;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.Views;

public partial class ManuelCalcOutWarehouseListView : ContentPage
{
    private readonly ManuelCalcOutWarehouseListViewModel _viewModel;

    public ManuelCalcOutWarehouseListView(ManuelCalcOutWarehouseListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}