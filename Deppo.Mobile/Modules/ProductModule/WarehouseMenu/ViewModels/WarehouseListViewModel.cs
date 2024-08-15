using System;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.ProductModule.WarehouseMenu.ViewModels;

public partial class WarehouseListViewModel : BaseViewModel
{
    private IHttpClientService _httpClientService;
    private readonly IWarehouseService _warehouseService;

    private readonly IUserDialogs _userDialogs;

    public WarehouseListViewModel(IWarehouseService warehouseService, IUserDialogs userDialogs, IHttpClientService httpClientService)
    {
        _warehouseService = warehouseService;
        _userDialogs = userDialogs;
        _httpClientService = httpClientService;

        Title = "Ambar Listesi";
    }
}
