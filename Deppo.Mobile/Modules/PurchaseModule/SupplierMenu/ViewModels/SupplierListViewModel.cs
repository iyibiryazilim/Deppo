using System;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.ViewModels;

public partial class SupplierListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ISupplierService _supplierService;
    private readonly IUserDialogs _userDialogs;

    public SupplierListViewModel(
        IHttpClientService httpClientService,
        ISupplierService supplierService,
        IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _supplierService = supplierService;
        _userDialogs = userDialogs;
    }

}
