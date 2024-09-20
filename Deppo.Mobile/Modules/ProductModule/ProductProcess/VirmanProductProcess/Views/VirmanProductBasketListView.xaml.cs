using Deppo.Mobile.Modules.ProductModule.ProductProcess.VirmanProductProcess.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.VirmanProductProcess.Views;

public partial class VirmanProductBasketListView : ContentPage
{
    private readonly VirmanProductBasketListViewModel _viewModel;

    public VirmanProductBasketListView(VirmanProductBasketListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
        _viewModel.CurrentPage = this;
    }
}