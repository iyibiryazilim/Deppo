using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.Models;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Core.Models.ProductModels;

public partial class WarehouseDetailModel : ObservableObject
{
	[ObservableProperty]
	private Warehouse warehouse = null!;

	[ObservableProperty]
	private double inputQuantity;

	[ObservableProperty]
	private double outputQuantity;

	//public ObservableCollection<WarehouseTransaction> LastTransactions { get; } = new();
}
