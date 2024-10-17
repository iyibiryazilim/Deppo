using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrder.ViewModels;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrder.Views;

public partial class WorkOrderProductListView : ContentPage
{
    private readonly WorkOrderProductListViewModel _viewModel;

    public WorkOrderProductListView(WorkOrderProductListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
        _viewModel.SearchText = searchBar;
    }
}