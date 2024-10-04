using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.ShipAddressModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.Views;
using DevExpress.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class InputProductPurchaseOrderProcessSupplierListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly IPurchaseSupplierService _purchaseSupplierService;
    private readonly IShipAddressService _shipAddressService;

    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    private PurchaseSupplier purchaseSupplier = null!;

    public InputProductPurchaseOrderProcessSupplierListViewModel(IHttpClientService httpClientService,

    IUserDialogs userDialogs,
    IPurchaseSupplierService purchaseSupplierService,
    IShipAddressService shipAddressService)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _purchaseSupplierService = purchaseSupplierService;
        _shipAddressService = shipAddressService;

        Title = "Tedarikçiler";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<PurchaseSupplier>(async (x) => await ItemTappedAsync(x));
        NextViewCommand = new Command(async () => await NextViewAsync());

        ShipAddressTappedCommand = new Command<ShipAddressModel>(async (shipAddress) => await ShipAddressTappedAsync(shipAddress));
        ConfirmShipAddressCommand = new Command(async () => await ConfirmShipAddressAsync());
        ShipAddressCloseCommand = new Command(async () => await ShipAddressCloseAsync());
        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
    }

    public ObservableCollection<PurchaseSupplier> Items { get; } = new();
    public ObservableCollection<ShipAddressModel> ShipAddresses { get; } = new();

    public Page CurrentPage { get; set; } = null!;

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command PerformSearchCommand { get; }
    public Command PerformEmptySearchCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command NextViewCommand { get; }

    public Command ConfirmShipAddressCommand { get; }
    public Command ShipAddressTappedCommand { get; }
    public Command ShipAddressCloseCommand { get; }

    [ObservableProperty]
    public SearchBar searchText;
    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);
            Items.Clear();

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _purchaseSupplierService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, SearchText.Text, 0, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                        Items.Add(Mapping.Mapper.Map<PurchaseSupplier>(item));

                    _userDialogs.HideHud();
                }
            }
        }
        catch (System.Exception ex)
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
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _purchaseSupplierService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, SearchText.Text, Items.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                        Items.Add(Mapping.Mapper.Map<PurchaseSupplier>(item));

                    _userDialogs.HideHud();
                }
            }
        }
        catch (System.Exception ex)
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

    private async Task ItemTappedAsync(PurchaseSupplier purchaseSupplier)
    {
        if (purchaseSupplier is null)
            return;
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            // Önceki seçimi kaldır
            if (PurchaseSupplier is not null)
            {
                PurchaseSupplier.IsSelected = false;
                PurchaseSupplier = null;
            }

            // Yeni öğeyi seç
            PurchaseSupplier = purchaseSupplier;
            PurchaseSupplier.IsSelected = true;

            // Ship adresleri varsa, bottom sheet aç
            if (purchaseSupplier.ShipAddressCount > 0)
            {
                await LoadShipAddressesAsync(purchaseSupplier);
                CurrentPage.FindByName<BottomSheet>("shipAddressBottomSheet").State = BottomSheetState.HalfExpanded;
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

    private async Task LoadShipAddressesAsync(PurchaseSupplier purchaseSupplier)
    {
        try
        {
            ShipAddresses.Clear();
            _userDialogs.Loading("Loading Ship Addresses...");
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _shipAddressService.GetObjectsByOrder(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                currentReferenceId: purchaseSupplier.ReferenceId,
                search: "",
                skip: 0,
                take: 99999
            );

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                    ShipAddresses.Add(Mapping.Mapper.Map<ShipAddressModel>(item));
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

    private async Task ShipAddressTappedAsync(ShipAddressModel shipAddressModel)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            ShipAddresses.ToList().ForEach(x => x.IsSelected = false);

            var selectedItem = ShipAddresses.FirstOrDefault(x => x.ReferenceId == shipAddressModel.ReferenceId);
            if (selectedItem != null)
                selectedItem.IsSelected = true;
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

    private async Task ConfirmShipAddressAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var selectedShipAddress = ShipAddresses.FirstOrDefault(x => x.IsSelected);
            if (selectedShipAddress is not null)
            {
                PurchaseSupplier.ShipAddressReferenceId = selectedShipAddress.ReferenceId;
                PurchaseSupplier.ShipAddressCode = selectedShipAddress.Code;
                CurrentPage.FindByName<BottomSheet>("shipAddressBottomSheet").State = BottomSheetState.Hidden;
            }
            else
            {
                _userDialogs.Alert("Lütfen bir adres seçiniz.", "Hata", "Tamam");
            }
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

    private async Task NextViewAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

			Console.WriteLine(WarehouseModel);
			Console.WriteLine(PurchaseSupplier);

            await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseOrderProcessBasketListView)}", new Dictionary<string, object>
            {
                [nameof(WarehouseModel)] = WarehouseModel,
                [nameof(PurchaseSupplier)] = PurchaseSupplier
            });
        }
        catch (System.Exception)
        {
            throw;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ShipAddressCloseAsync()
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            CurrentPage.FindByName<BottomSheet>("shipAddressBottomSheet").State = BottomSheetState.Hidden;
        });
    }

    private async Task PerformSearchAsync()
    {
        if (IsBusy)
            return;

        try
        {
            if (string.IsNullOrWhiteSpace(SearchText.Text))
            {
                await LoadItemsAsync();
                SearchText.Unfocus();
                return;
            }
            IsBusy = true;

            Items.Clear();

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _purchaseSupplierService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, SearchText.Text, 0, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                        Items.Add(Mapping.Mapper.Map<PurchaseSupplier>(item));

                    _userDialogs.HideHud();
                }
            }
            if (!result.IsSuccess)
            {
                _userDialogs.Alert(result.Message, "Hata");
                return;
            }
        }
        catch (System.Exception ex)
        {
            _userDialogs.Alert(ex.Message, "Hata");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task PerformEmptySearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText.Text))
        {
            await PerformSearchAsync();
        }
    }
}