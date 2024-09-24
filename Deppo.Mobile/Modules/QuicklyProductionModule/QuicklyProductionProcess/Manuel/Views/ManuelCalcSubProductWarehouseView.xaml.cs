using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.ViewModels;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.Views;

public partial class ManuelCalcSubProductWarehouseView : ContentPage
{
    private readonly ManuelCalcSubProductWarehouseViewModel _viewModel;

    public ManuelCalcSubProductWarehouseView(ManuelCalcSubProductWarehouseViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}