using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionPanel.ViewModels;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionPanel.Views;

public partial class QuicklyProductionPanelView : ContentPage
{
	private readonly QuicklyProductionPanelViewModel _viewModel;
	public QuicklyProductionPanelView(QuicklyProductionPanelViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}