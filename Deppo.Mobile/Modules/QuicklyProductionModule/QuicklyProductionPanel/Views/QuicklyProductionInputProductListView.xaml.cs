using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionPanel.ViewModels;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionPanel.Views;

public partial class QuicklyProductionInputProductListView : ContentPage
{
	private readonly QuicklyProductionInputProductListViewModel _viewModel;
	public QuicklyProductionInputProductListView(QuicklyProductionInputProductListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;	
	}
}