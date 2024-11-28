using CommunityToolkit.Mvvm.ComponentModel;

namespace Deppo.Mobile.Modules.ResultModule;

public partial class ResultModel : ObservableObject
{
	[ObservableProperty]
	string pageTitle = string.Empty;
	
	[ObservableProperty]
	string message = string.Empty;

	[ObservableProperty]
	string errorMessage = string.Empty;

	[ObservableProperty]
	string code = string.Empty;

	[ObservableProperty]
	int pageCountToBack = 0;

	[ObservableProperty]
    bool isSuccess = false;

}
