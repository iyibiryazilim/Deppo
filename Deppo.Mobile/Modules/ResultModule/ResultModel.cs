using CommunityToolkit.Mvvm.ComponentModel;

namespace Deppo.Mobile.Modules.ResultModule;

public partial class ResultModel : ObservableObject
{
	[ObservableProperty]
	string pageTitle = string.Empty;
	
	[ObservableProperty]
	string message = string.Empty;

	[ObservableProperty]
	int pageCountToBack;

}
