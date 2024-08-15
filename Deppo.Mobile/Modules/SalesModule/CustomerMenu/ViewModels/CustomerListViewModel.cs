using System;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.SalesModule.CustomerMenu.ViewModels;

public partial class CustomerListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ICustomerService _customerService;
    private readonly IUserDialogs _userDialogs;

    public CustomerListViewModel(
        IHttpClientService httpClientService,
        ICustomerService customerService,
        IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _customerService = customerService;
        _userDialogs = userDialogs;
    }

}
