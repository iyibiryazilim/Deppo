using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.SeriLotModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.VirmanModule.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class VirmanProductOutListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ISeriLotTransactionService _serilotTransactionService;
    private readonly IUserDialogs _userDialogs;
    private readonly IWarehouseTotalService _warehouseTotalService;

    public VirmanProductOutListViewModel(IHttpClientService httpClientService, ISeriLotTransactionService serilotTransactionService, IUserDialogs userDialogs, IWarehouseTotalService warehouseTotalService)
    {
        _httpClientService = httpClientService;
        _serilotTransactionService = serilotTransactionService;
        _userDialogs = userDialogs;
        _warehouseTotalService = warehouseTotalService;
    }

    public Page CurrentPage { get; set; }
}