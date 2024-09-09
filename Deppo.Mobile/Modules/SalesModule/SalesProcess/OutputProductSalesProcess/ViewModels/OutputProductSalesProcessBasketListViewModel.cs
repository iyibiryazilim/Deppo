using Controls.UserDialogs.Maui;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.ViewModels;

public partial class OutputProductSalesProcessBasketListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IUserDialogs _userDialogs;

	public OutputProductSalesProcessBasketListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
	}
}
