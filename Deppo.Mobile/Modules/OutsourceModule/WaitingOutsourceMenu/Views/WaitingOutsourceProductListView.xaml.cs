using Deppo.Mobile.Modules.OutsourceModule.WaitingOutsourceMenu.ViewModels;

namespace Deppo.Mobile.Modules.OutsourceModule.WaitingOutsourceMenu.Views;

public partial class WaitingOutsourceProductListView : ContentPage
{
    private readonly WaitingOutsourceProductListViewModel _viewModel;

    public WaitingOutsourceProductListView(WaitingOutsourceProductListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
        _viewModel.SearchText = searchBar;
    }
}