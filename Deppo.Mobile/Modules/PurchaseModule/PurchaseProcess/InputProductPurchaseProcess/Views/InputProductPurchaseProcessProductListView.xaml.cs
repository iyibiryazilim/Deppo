using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.Views;

public partial class InputProductPurchaseProcessProductListView : ContentPage
{
    private readonly InputProductPurchaseProcessProductListViewModel _viewModel;

    public InputProductPurchaseProcessProductListView(InputProductPurchaseProcessProductListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.SearchText = searchBar;
        _viewModel.CurrentPage = this;
    }
}