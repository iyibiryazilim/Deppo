using Deppo.Mobile.Modules.ProductModule.ProductProcess.DemandProcess.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.DemandProcess.Views;

public partial class DemandProcessProductListView : ContentPage
{
	private readonly DemandProcessProductListViewModel _viewModel;
    public DemandProcessProductListView(DemandProcessProductListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.SearchText = searchBar;
        _viewModel.CurrentPage = this;
    }

}