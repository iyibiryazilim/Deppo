using CommunityToolkit.Mvvm.ComponentModel;

namespace Deppo.Mobile.Core.Models.ProcurementModels.ByCustomerModels
{
	public partial class ProcurementCustomerFormModel : ObservableObject
	{
		[ObservableProperty]
		List<ProcurementCustomerBasketProductModel> products = new();
	}
}
