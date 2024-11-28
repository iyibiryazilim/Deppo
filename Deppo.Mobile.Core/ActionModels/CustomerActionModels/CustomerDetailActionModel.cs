using CommunityToolkit.Mvvm.ComponentModel;

namespace Deppo.Mobile.Core.ActionModels.CustomerActionModels;

public partial class CustomerDetailActionModel : ObservableObject
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

	public CustomerDetailActionModel()
	{

	}
}
