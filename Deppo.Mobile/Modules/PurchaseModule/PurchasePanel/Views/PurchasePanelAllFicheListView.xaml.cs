using Deppo.Mobile.Modules.PurchaseModule.PurchasePanel.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchasePanel.Views;

public partial class PurchasePanelAllFicheListView : ContentPage
{
	private readonly PurchasePanelAllFicheListViewModel _viewModel;
    public PurchasePanelAllFicheListView(PurchasePanelAllFicheListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }

}