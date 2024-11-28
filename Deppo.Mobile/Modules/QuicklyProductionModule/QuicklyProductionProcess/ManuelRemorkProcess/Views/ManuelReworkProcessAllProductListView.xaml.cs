using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.ViewModels;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.Views;

public partial class ManuelReworkProcessAllProductListView : ContentPage
{
	private readonly ManuelReworkProcessAllProductListViewModel _viewModel;
	public ManuelReworkProcessAllProductListView(ManuelReworkProcessAllProductListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
		_viewModel.SearchText = searchBar;
	}
}