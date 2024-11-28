using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrderReworkProcess.ViewModels;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrderReworkProcess.Views;

public partial class WorkOrderReworkProcessSubProductLocationListView : ContentPage
{
	private readonly WorkOrderReworkProcessSubProductLocationListViewModel _viewModel;
	public WorkOrderReworkProcessSubProductLocationListView(WorkOrderReworkProcessSubProductLocationListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}