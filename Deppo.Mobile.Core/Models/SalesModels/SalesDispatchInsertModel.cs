using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.SalesModels;

public partial class SalesDispatchInsertModel : ObservableObject
{
	[ObservableProperty]
	bool isSelected;

	[ObservableProperty]
	string typeName = string.Empty;
}
