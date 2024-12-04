using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Kotlin.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.OutsourceModels.BasketModels;

public partial class InputOutsourceTransferV2BasketModel : ObservableObject
{
	[ObservableProperty]
	WarehouseModel? outsourceWarehouseModel;

	[ObservableProperty]
	OutsourceModel? outsourceModel;

	[ObservableProperty]
	InputOutsourceTransferProductModel? inputOutsourceTransferMainProductModel;
}
