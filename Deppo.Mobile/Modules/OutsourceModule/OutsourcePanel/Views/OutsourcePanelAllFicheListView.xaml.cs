using Deppo.Mobile.Modules.OutsourceModule.OutsourcePanel.ViewModels;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourcePanel.Views;

public partial class OutsourcePanelAllFicheListView : ContentPage
{
    private readonly OutsourcePanelAllFicheListViewModel _viewModel;

    public OutsourcePanelAllFicheListView(OutsourcePanelAllFicheListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}