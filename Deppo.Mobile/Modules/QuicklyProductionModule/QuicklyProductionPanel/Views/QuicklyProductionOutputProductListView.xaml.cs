using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionPanel.ViewModels;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionPanel.Views;

public partial class QuicklyProductionOutputProductListView : ContentPage
{
	private readonly QuicklyProductionOutputProductListViewModel _viewModel;
	public QuicklyProductionOutputProductListView(QuicklyProductionOutputProductListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}