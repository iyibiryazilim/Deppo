using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcess.OutputOutsourceTransfer.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class OutputOutsourceTransferBasketListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ILocationTransactionService _locationTransactionService;
    private readonly ISeriLotTransactionService _seriLotTransactionService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    WarehouseModel warehouseModel = null!;

    public OutputOutsourceTransferBasketListViewModel(
        IHttpClientService httpClientService,
        IUserDialogs userDialogs,
        ILocationTransactionService locationTransactionService,
        ISeriLotTransactionService seriLotTransactionService)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _locationTransactionService = locationTransactionService;
        _seriLotTransactionService = seriLotTransactionService;

        Title = "Fason Çıkış Transfer Sepeti";
    }

    public Page CurrentPage { get; set; }
}
