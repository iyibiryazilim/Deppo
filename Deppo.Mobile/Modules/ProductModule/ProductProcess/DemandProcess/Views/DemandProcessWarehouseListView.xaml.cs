using Deppo.Mobile.Modules.ProductModule.ProductProcess.DemandProcess.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.DemandProcess.Views;

public partial class DemandProcessWarehouseListView : ContentPage
{
	private readonly DemandProcessWarehouseListViewModel _viewModel;
    public DemandProcessWarehouseListView(DemandProcessWarehouseListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

}