using System;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.SalesModule.WaitingOrderMenu.ViewModels;

public partial class WaitingSalesOrderListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IWaitingSalesOrderService _waitingSalesOrderService;
    private readonly IUserDialogs _userDialogs;

    public WaitingSalesOrderListViewModel(
        IHttpClientService httpClientService,
        IWaitingSalesOrderService waitingSalesOrderService,
        IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _waitingSalesOrderService = waitingSalesOrderService;
        _userDialogs = userDialogs;
    }

}
