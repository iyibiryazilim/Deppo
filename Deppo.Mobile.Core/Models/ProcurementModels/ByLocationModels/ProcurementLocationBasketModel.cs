using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Core.Models.SalesModels;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Core.Models.ProcurementModels.ByLocationModels
{
	public partial class ProcurementLocationBasketModel : ObservableObject
	{
		[ObservableProperty]
		ProcurementByLocationProduct procurementByLocationProduct;

		[ObservableProperty]
		public ObservableCollection<ProcurementCustomerModel> procurementCustomers = new();
	}
}
