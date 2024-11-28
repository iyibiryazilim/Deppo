using Deppo.Mobile.Modules.PurchaseModule.PurchasePanel.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchasePanel.Views;

public partial class PurchasePanelView : ContentPage
{
	private readonly PurchasePanelViewModel _viewModel;
	public PurchasePanelView(PurchasePanelViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}