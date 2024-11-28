using Deppo.Mobile.Modules.CountingModule.CountingPanel.ViewModels;

namespace Deppo.Mobile.Modules.CountingModule.CountingPanel.Views;

public partial class CountingPanelView : ContentPage
{
	private readonly CountingPanelViewModel _viewModel;
	public CountingPanelView(CountingPanelViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}