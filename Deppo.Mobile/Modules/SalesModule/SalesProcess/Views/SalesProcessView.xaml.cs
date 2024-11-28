using Deppo.Mobile.Modules.SalesModule.SalesProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.Views;

public partial class SalesProcessView : ContentPage
{
    private readonly SalesProcessViewModel _viewModel;

    public SalesProcessView(SalesProcessViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
        _viewModel.CurrentPage = this;
    }
}