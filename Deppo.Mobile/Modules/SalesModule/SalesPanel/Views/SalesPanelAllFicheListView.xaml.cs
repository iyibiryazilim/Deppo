using Deppo.Mobile.Modules.SalesModule.SalesPanel.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesPanel.Views;

public partial class SalesPanelAllFicheListView : ContentPage
{
	private readonly SalesPanelAllFicheListViewModel _viewModel;
    public SalesPanelAllFicheListView(SalesPanelAllFicheListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }

}