using Deppo.Mobile.Modules.ProductModule.ProductProcess.VirmanProductProcess.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.VirmanProductProcess.Views;

public partial class VirmanProductFormListView : ContentPage
{
    private readonly VirmanProductFormListViewModel _viewModel;

    public VirmanProductFormListView(VirmanProductFormListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}