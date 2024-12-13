using Deppo.Mobile.Modules.VariantModule.ViewModels;

namespace Deppo.Mobile.Modules.VariantModule.Views;

public partial class VariantListView : ContentPage
{
	private readonly VariantListViewModel _viewModel;
	public VariantListView(VariantListViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
        _viewModel.SearchText = searchBar;

    }
}