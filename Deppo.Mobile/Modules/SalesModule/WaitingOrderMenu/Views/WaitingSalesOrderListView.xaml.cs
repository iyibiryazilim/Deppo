using Deppo.Mobile.Modules.SalesModule.WaitingOrderMenu.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.WaitingOrderMenu.Views;

public partial class WaitingSalesOrderListView : ContentPage
{
    private readonly WaitingSalesOrderListViewModel _viewModel;

    public WaitingSalesOrderListView(WaitingSalesOrderListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}