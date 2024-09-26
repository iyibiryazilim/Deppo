using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.ViewModels;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.Views;

public partial class ManuelFormListView : ContentPage
{
    private readonly ManuelFormListViewModel _viewModel;

    public ManuelFormListView(ManuelFormListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
        _viewModel.CurrentPage= this;
    }
}