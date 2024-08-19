using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.ProductModels;

public partial class WarehouseDetailModel : ObservableObject
{
	[ObservableProperty]
	private Warehouse warehouse = null!;
}
