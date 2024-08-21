using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.ProductModule.WarehouseMenu.ViewModels;

public partial class WarehouseInputTransactionViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly ICustomQueryService _customQueryService;
	private readonly IUserDialogs _userDialogs;
	public WarehouseInputTransactionViewModel(IHttpClientService httpClientService, ICustomQueryService customQueryService, IUserDialogs userDialogs)
    {
        Title = "Ambar Çıkış Hareketleri";
		_httpClientService = httpClientService;
		_customQueryService = customQueryService;
		_userDialogs = userDialogs;
    }
}
