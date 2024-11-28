using Deppo.Mobile.Modules.SalesModule.CustomerMenu.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.CustomerMenu.Views;

public partial class CustomerListView : ContentPage
{
    private readonly CustomerListViewModel _viewModel;

    public CustomerListView(CustomerListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.SearchText = searchBar;
    }
}