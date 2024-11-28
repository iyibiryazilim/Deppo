using System;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.FastProductionModule.ViewModels;

public partial class FastProductionProductViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;

    public FastProductionProductViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;

        Title = "Hızlı Üretim";
    }

}
