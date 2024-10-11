using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrderReworkProcess.ViewModels;

public partial class WorkOrderReworkProcessBasketViewModel : BaseViewModel
{

	public WorkOrderReworkProcessBasketViewModel()
	{
		Title = "Sepet";
	}
	public Page CurrentPage { get; set; } = null!;
}
