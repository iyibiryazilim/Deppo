using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.PurchaseModule.WaitingOrderMenu.ViewModels;

public partial class WaitingPurchaseOrderListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly ISupplierService _supplierService;
    private readonly IWaitingPurchaseOrderService _waitingPurchaseOrderService;

    public ObservableCollection<Supplier> Suppliers { get; } = new();
    public ObservableCollection<WaitingPurchaseOrder> Items { get; } = new();

    [ObservableProperty]
    private PurchaseSupplier purchaseSupplier = null!;

    [ObservableProperty]
    private Supplier? selectedSupplier;


    public WaitingPurchaseOrderListViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService, IWaitingPurchaseOrderService waitingPurchaseOrderService, ISupplierService supplierService)
    {
        _userDialogs = userDialogs;
        _httpClientService = httpClientService;
        _waitingPurchaseOrderService = waitingPurchaseOrderService;
        _supplierService = supplierService;

        Title = "Bekleyen Satın Alma Siparişleri";

        LoadItemsCommand = new Command<Supplier>(async (x) => await LoadItemsAsync(x));
        LoadSupplierCommand = new Command(async () => await LoadSupplierAsync());
        OnSelectedSupplierChangeCommand = new Command<Supplier>(async (x) => await OnSelectedSupplierChangedAsync(x));
    }

    public Command LoadItemsCommand { get; }

    public Command LoadSupplierCommand { get; }

    public Command<Supplier?> OnSelectedSupplierChangeCommand { get; }




    private async Task OnSelectedSupplierChangedAsync(Supplier? value)
    {
        if (value is not null)
        {
            await LoadItemsAsync(value);
        }
    }

    public async Task LoadSupplierAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            _userDialogs.Loading("Yükleniyor");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            await Task.Delay(1000);
            var result = await _supplierService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, string.Empty, Suppliers.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                    Suppliers.Add(Mapping.Mapper.Map<Supplier>(item));

                _userDialogs.Loading().Hide();
            }
            else
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.Loading().Hide();

                _userDialogs.Alert(message: result.Message, title: "Load Items");
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(message: ex.Message, title: "Load Items Error");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task LoadItemsAsync(Supplier supplier)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor..");
            Items.Clear();
            await Task.Delay(10000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _waitingPurchaseOrderService.GetObjectsBySupplier(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, supplier.ReferenceId, string.Empty, Items.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                        Items.Add(Mapping.Mapper.Map<WaitingPurchaseOrder>(item));
                }
            }

            _userDialogs.HideHud();
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }
}