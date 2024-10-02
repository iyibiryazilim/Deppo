using Deppo.Mobile.Modules.ProductModule.ProductProcess.VirmanProductProcess.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.VirmanProductProcess.Views;

public partial class VirmanProductOutListView : ContentPage
{
    private readonly VirmanProductOutListViewModel _viewModel;

    public VirmanProductOutListView(VirmanProductOutListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
        _viewModel.CurrentPage = this;
        _viewModel.SearchText = searchBar;
    }
}