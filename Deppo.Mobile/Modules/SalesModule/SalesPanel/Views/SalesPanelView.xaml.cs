using Deppo.Mobile.Modules.SalesModule.SalesPanel.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesPanel.Views;

public partial class SalesPanelView : ContentPage
{
	private readonly SalesPanelViewModel _viewModel;
	public SalesPanelView(SalesPanelViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}