using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.ActionModels.SupplierActionModels;

public partial class SupplierDetailActionModel : ObservableObject
{
	[ObservableProperty]
	string actionName = string.Empty;

	[ObservableProperty]
	string actionUrl = string.Empty;

	[ObservableProperty]
	int lineNumber = 0;

	[ObservableProperty]
	string icon = string.Empty;

	[ObservableProperty]
	bool isSelected = false;
    public SupplierDetailActionModel()
    {
        
    }
}
