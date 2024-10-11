using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.ShipAddressModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.ViewModels;

[QueryProperty(name:nameof(WarehouseModel),queryId:nameof(WarehouseModel))]
public partial class ProcurementByCustomerListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IProcurementByCustomerService _procurementByCustomerService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    WarehouseModel? warehouseModel;

    public ProcurementByCustomerListViewModel(
        IHttpClientService httpClientService,
        IProcurementByCustomerService procurementByCustomerService,
        IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _procurementByCustomerService = procurementByCustomerService;
        _userDialogs = userDialogs;

        Title = "Siparişi Olan Müşteriler";
    }

    public ObservableCollection<CustomerModel> Items { get; } = new();
    public ObservableCollection<ShipAddressModel> ShipAddressItems { get; } = new();

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command<CustomerModel> ItemTappedCommand { get; }
}
