using Deppo.Mobile.Modules.SalesModule.SalesPanel.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesPanel.Views;

public partial class SalesPanelShippedProductListView : ContentPage
{
	private readonly SalesPanelShippedProductListViewModel _viewModel;
    public SalesPanelShippedProductListView(SalesPanelShippedProductListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}