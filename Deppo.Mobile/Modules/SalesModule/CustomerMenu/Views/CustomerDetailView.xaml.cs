using Deppo.Mobile.Modules.SalesModule.CustomerMenu.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.CustomerMenu.Views;

public partial class CustomerDetailView : ContentPage
{
    public readonly CustomerDetailViewModel _viewModel;

    public CustomerDetailView(CustomerDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}