using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrderReworkProcess.ViewModels;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrderReworkProcess.Views;

public partial class WorkOrderReworkProcessFormView : ContentPage
{
	private readonly WorkOrderReworkProcessFormViewModel _viewModel;
	public WorkOrderReworkProcessFormView(WorkOrderReworkProcessFormViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}