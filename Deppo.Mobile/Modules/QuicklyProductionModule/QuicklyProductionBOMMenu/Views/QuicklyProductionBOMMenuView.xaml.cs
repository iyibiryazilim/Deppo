using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionBOMMenu.ViewModels;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionBOMMenu.Views;

public partial class QuicklyProductionBOMMenuView : ContentPage
{
    private readonly QuicklyProductionBOMMenuViewModel _viewModel;

    public QuicklyProductionBOMMenuView(QuicklyProductionBOMMenuViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
        _viewModel.SearchText = searchBar;
    }
}