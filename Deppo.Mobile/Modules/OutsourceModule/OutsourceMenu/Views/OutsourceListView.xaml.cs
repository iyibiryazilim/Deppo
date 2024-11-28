using Deppo.Mobile.Modules.OutsourceModule.OutsourceMenu.ViewModels;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceMenu.Views;

public partial class OutsourceListView : ContentPage
{
	private readonly OutsourceListViewModel _viewModel;
	public OutsourceListView(OutsourceListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.SearchText = searchBar;
	}
}