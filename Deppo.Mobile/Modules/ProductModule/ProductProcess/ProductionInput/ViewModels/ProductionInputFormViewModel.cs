using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.ProductionInput.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(SelectedItems), queryId: nameof(SelectedItems))]
public partial class ProductionInputFormViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    ObservableCollection<WarehouseTotalModel> selectedItems = new();

    [ObservableProperty]
    WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    DateTime transactionDate = DateTime.Now;

    [ObservableProperty]
    string documentNumber = string.Empty;

    public ProductionInputFormViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;

        Title = "Üretim Giriş Formu";
    }

}
