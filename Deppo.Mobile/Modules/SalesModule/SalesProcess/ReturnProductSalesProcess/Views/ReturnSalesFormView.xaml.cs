using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesProcess.Views;

public partial class ReturnSalesFormView : ContentPage
{
    private readonly ReturnSalesFormViewModel _viewModel;

    public ReturnSalesFormView(ReturnSalesFormViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}