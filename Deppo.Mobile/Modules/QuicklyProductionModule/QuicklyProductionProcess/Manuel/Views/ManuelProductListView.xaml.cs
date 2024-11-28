using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.ViewModels;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.Views;

public partial class ManuelProductListView : ContentPage
{
    private readonly ManuelProductListViewModel _viewModel;

    public ManuelProductListView(ManuelProductListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
        _viewModel.SearchText = searchBar;
    }
}