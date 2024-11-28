using Deppo.Mobile.Modules.SalesModule.CustomerMenu.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.CustomerMenu.Views;

public partial class CustomerInputTransactionView : ContentPage
{
    private readonly CustomerInputTransactionViewModel _viewModel;

    public CustomerInputTransactionView(CustomerInputTransactionViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.SearchText = searchBar;
    }
}