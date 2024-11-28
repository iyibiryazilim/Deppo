using Deppo.Mobile.Modules.SalesModule.SalesPanel.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesPanel.Views;

public partial class SalesPanelWaitingProductListView : ContentPage
{
	private readonly SalesPanelWaitingProductListViewModel _viewModel;
    public SalesPanelWaitingProductListView(SalesPanelWaitingProductListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}