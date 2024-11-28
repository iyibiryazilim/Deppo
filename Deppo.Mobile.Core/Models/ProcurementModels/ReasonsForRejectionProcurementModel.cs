using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.ProcurementModels;

public partial class ReasonsForRejectionProcurementModel : ObservableObject
{
	[ObservableProperty]
	Guid oid;

	[ObservableProperty]
	string code = string.Empty;

	[ObservableProperty]
	string name = string.Empty;

	[ObservableProperty]
	bool isSelected = false;
}
