using Deppo.Mobile.Modules.SalesModule.CustomerMenu.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.CustomerMenu.Views;

public partial class CustomerOutputTransactionView : ContentPage
{
    private readonly CustomerOutputTransactionViewModel _viewModel;

    public CustomerOutputTransactionView(CustomerOutputTransactionViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.SearchText = searchBar;
    }
}