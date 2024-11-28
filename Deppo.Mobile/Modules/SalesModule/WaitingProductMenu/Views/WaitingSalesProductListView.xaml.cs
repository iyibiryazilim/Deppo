using Deppo.Mobile.Modules.SalesModule.WaitingProductMenu.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.WaitingProductMenu.Views;

public partial class WaitingSalesProductListView : ContentPage
{
	private readonly WaitingSalesProductListViewModel _viewModel;
	public WaitingSalesProductListView(WaitingSalesProductListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
		_viewModel.CurrentPage = this;
        _viewModel.SearchText = searchBar;
    }
}