using Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.Views;

public partial class ProductDetailAllFicheListView : ContentPage
{
    private readonly ProductDetailAllFicheListViewModel _viewModel;

    public ProductDetailAllFicheListView(ProductDetailAllFicheListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}