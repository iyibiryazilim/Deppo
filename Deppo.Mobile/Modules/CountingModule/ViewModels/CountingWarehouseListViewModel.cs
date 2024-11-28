using System;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.CountingModule.ViewModels;

public partial class CountingWarehouseListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;

    public CountingWarehouseListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;

        Title = "Ambar Seçimi";
    }

}
