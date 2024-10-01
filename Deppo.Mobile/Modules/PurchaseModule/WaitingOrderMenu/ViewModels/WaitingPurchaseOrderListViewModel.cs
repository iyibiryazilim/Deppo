using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;

namespace Deppo.Mobile.Modules.PurchaseModule.WaitingOrderMenu.ViewModels;

public partial class WaitingPurchaseOrderListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly ISupplierService _supplierService;
    private readonly IWaitingPurchaseOrderService _waitingPurchaseOrderService;

    public ObservableCollection<SupplierModel> Suppliers { get; } = new();
    public ObservableCollection<WaitingPurchaseOrder> Items { get; } = new();

    [ObservableProperty]
    private SupplierModel selectedSupplierModel;

    [ObservableProperty]
    private SupplierModel supplierModel = null!;

    public Page CurrentPage { get; set; }

    public WaitingPurchaseOrderListViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService, IWaitingPurchaseOrderService waitingPurchaseOrderService, ISupplierService supplierService)
    {
        _userDialogs = userDialogs;
        _httpClientService = httpClientService;
        _waitingPurchaseOrderService = waitingPurchaseOrderService;
        _supplierService = supplierService;

        Title = "Bekleyen Satın Alma Siparişleri";

        LoadItemsCommand = new Command<Supplier>(async (x) => await LoadItemsAsync(x));
        LoadSupplierCommand = new Command(async () => await LoadSupplierAsync());
        ItemTappedCommand = new Command<SupplierModel>(ItemTappedAsync);
        SupplierConfirmCommand = new Command(async () => await SupplierConfirmAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
    }

    public Command LoadItemsCommand { get; }

    public Command LoadSupplierCommand { get; }

    public Command LoadMoreItemsCommand { get; }
    public Command ItemTappedCommand { get; }

    public Command SupplierConfirmCommand { get; }

    public async Task LoadSupplierAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            _userDialogs.Loading("Yükleniyor");
            await Task.Delay(1000);
            Suppliers.Clear();
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _supplierService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, string.Empty, 0, 999999);
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                    Suppliers.Add(Mapping.Mapper.Map<SupplierModel>(item));

                CurrentPage.FindByName<BottomSheet>("supplierBottomSheet").State = BottomSheetState.HalfExpanded;

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
        if (supplier is null)
            return;
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            Items.Clear();

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

    private async Task LoadMoreItemsAsync()
    {
        if (SelectedSupplierModel is null)
            return;
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.Loading("Loading More Items...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _waitingPurchaseOrderService.GetObjectsBySupplier(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, supplierReferenceId: SelectedSupplierModel.ReferenceId, "", Items.Count, 20);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    Items.Add(Mapping.Mapper.Map<WaitingSalesOrder>(item));
                }
            }

            _userDialogs.HideHud();
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void ItemTappedAsync(SupplierModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            Suppliers.ToList().ForEach(x => x.IsSelected = false);

            var selectedItem = Suppliers.FirstOrDefault(x => x.ReferenceId == item.ReferenceId);
            if (selectedItem != null)
                selectedItem.IsSelected = true;

            SelectedSupplierModel = item;
        }
        catch (Exception ex)
        {
            _userDialogs.Alert(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SupplierConfirmAsync()
    {
        CurrentPage.FindByName<BottomSheet>("supplierBottomSheet").State = BottomSheetState.Hidden;

        Title = string.Format("{0} Bekleyen Siparişleri", SelectedSupplierModel.Name);
        await LoadItemsAsync(SelectedSupplierModel);
    }
}