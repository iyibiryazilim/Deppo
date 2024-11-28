using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Core.Models.SalesModels;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Core.Models.ProcurementModels.ByLocationModels
{
	public partial class ProcurementCustomerBasketModel : ObservableObject
	{
		[ObservableProperty]
		ProcurementCustomerModel? procurementCustomerModel;

		[ObservableProperty]
		ObservableCollection<ProcurementByLocationProduct> procurementByLocationProducts = new();

		[ObservableProperty]
		bool isSelected;

	}
}
