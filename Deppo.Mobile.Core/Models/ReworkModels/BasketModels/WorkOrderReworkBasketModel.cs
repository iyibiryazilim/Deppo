using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Core.Models.QuicklyModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.ReworkModels.BasketModels;

/// <summary>
///  Used for WorkOrderRework
/// </summary>
public partial class WorkOrderReworkBasketModel : ObservableObject
{

	[ObservableProperty]
	QuicklyBOMProductModel quicklyBomProduct = null!;
}
