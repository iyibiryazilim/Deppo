using Deppo.Mobile.Modules.CountingModule.NegativeProductMenu.ViewModels;

namespace Deppo.Mobile.Modules.CountingModule.NegativeProductMenu.Views;

public partial class NegativeProductListView : ContentPage
{
	private readonly NegativeProductListViewModel _viewModel;
	public NegativeProductListView(NegativeProductListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}