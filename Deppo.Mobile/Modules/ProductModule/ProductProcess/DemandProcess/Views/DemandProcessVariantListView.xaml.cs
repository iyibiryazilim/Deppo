using Deppo.Mobile.Modules.ProductModule.ProductProcess.DemandProcess.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.DemandProcess.Views;

public partial class DemandProcessVariantListView : ContentPage
{
	private readonly DemandProcessVariantListViewModel _viewModel;
    public DemandProcessVariantListView(DemandProcessVariantListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.SearchText = searchBar;
        _viewModel.CurrentPage = this;
    }

}