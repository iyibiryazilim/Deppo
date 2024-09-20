using Deppo.Mobile.Modules.ProductModule.ProductProcess.VirmanProductProcess.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.VirmanProductProcess.Views;

public partial class VirmanProductInListView : ContentPage
{
    private readonly VirmanProductInListViewModel _viewModel;

    public VirmanProductInListView(VirmanProductInListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
        _viewModel.CurrentPage = this;
    }
}