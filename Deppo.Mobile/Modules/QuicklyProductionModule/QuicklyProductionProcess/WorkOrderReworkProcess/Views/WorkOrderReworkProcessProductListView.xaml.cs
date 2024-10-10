using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrderReworkProcess.ViewModels;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrderReworkProcess.Views;

public partial class WorkOrderReworkProcessProductListView : ContentPage
{
	private readonly WorkOrderReworkProcessProductListViewModel _viewModel;
	public WorkOrderReworkProcessProductListView(WorkOrderReworkProcessProductListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.SearchText = searchBar;
	}
}