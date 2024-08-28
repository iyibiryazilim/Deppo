using Controls.UserDialogs.Maui;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.ViewModels;

public partial class OutputProductProcessFormViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IUserDialogs _userDialogs;
	public OutputProductProcessFormViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;

		Title = "Form";
	}
}
