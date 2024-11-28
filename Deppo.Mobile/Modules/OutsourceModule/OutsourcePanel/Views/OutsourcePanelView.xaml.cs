using Deppo.Mobile.Modules.OutsourceModule.OutsourcePanel.ViewModels;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourcePanel.Views;

public partial class OutsourcePanelView : ContentPage
{
	private readonly OutsourcePanelViewModel _viewModel;
	public OutsourcePanelView(OutsourcePanelViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}