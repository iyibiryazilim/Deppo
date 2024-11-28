using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionPanel.ViewModels;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionPanel.Views;

public partial class QuicklyProductionPanelAllFicheListView : ContentPage
{
	private readonly QuicklyProductionPanelAllFicheListViewModel _viewModel;
    public QuicklyProductionPanelAllFicheListView(QuicklyProductionPanelAllFicheListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;

	}
}