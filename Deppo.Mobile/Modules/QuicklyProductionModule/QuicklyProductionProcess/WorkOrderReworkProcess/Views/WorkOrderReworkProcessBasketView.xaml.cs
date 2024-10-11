using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrderReworkProcess.ViewModels;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrderReworkProcess.Views;

public partial class WorkOrderReworkProcessBasketView : ContentPage
{
	private readonly WorkOrderReworkProcessBasketViewModel _viewModel;
	public WorkOrderReworkProcessBasketView(WorkOrderReworkProcessBasketViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}