using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Core.Models.WarehouseModels;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Core.Models.OutsourceModels.BasketModels;

public partial class InputOutsourceTransferV2BasketModel : ObservableObject
{
	[ObservableProperty]
	WarehouseModel? outsourceWarehouseModel;

	[ObservableProperty]
	OutsourceModel? outsourceModel;

	[ObservableProperty]
	InputOutsourceTransferProductModel? inputOutsourceTransferMainProductModel;

	[ObservableProperty]
	ObservableCollection<InputOutsourceTransferSubProductModel>? inputOutsourceTransferSubProducts = new();
}
