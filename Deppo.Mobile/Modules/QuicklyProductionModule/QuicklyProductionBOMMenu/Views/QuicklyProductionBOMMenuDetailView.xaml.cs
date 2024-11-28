using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionBOMMenu.ViewModels;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionBOMMenu.Views;

public partial class QuicklyProductionBOMMenuDetailView : ContentPage
{
    private readonly QuicklyProductionBOMMenuDetailViewModel _viewModel;

    public QuicklyProductionBOMMenuDetailView(QuicklyProductionBOMMenuDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}