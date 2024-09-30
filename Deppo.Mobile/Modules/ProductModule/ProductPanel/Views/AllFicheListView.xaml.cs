using Deppo.Mobile.Modules.ProductModule.ProductPanel.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductPanel.Views;

public partial class AllFicheListView : ContentPage
{
	private readonly AllFicheListViewModel _viewModel;
	public AllFicheListView(AllFicheListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}