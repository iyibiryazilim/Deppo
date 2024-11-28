using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrder.ViewModels;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrder.Views;

public partial class WorkOrderProductLocationListView : ContentPage
{
	private readonly WorkOrderProductLocationListViewModel _viewModel;
	public WorkOrderProductLocationListView(WorkOrderProductLocationListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}