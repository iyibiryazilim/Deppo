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
using DevExpress.Maui.Core.Internal;
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

    [ObservableProperty]
    private ShipAddressModel selectedShipAddressModel;

    public ObservableCollection<PurchaseSupplier> Items { get; } = new();
    public ObservableCollection<ShipAddressModel> ShipAddresses { get; } = new();

    [ObservableProperty]
    public SearchBar searchText;

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
        BackCommand = new Command(async () => await BackAsyc());

        ShipAddressTappedCommand = new Command<ShipAddressModel>(async (shipAddress) => await ShipAddressTappedAsync(shipAddress));
        ConfirmShipAddressCommand = new Command(async () => await ConfirmShipAddressAsync());
        ShipAddressCloseCommand = new Command(async () => await ShipAddressCloseAsync());
        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
    }

    public Page CurrentPage { get; set; } = null!;

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command PerformSearchCommand { get; }
    public Command PerformEmptySearchCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command NextViewCommand { get; }
    public Command BackCommand { get; }

    public Command ConfirmShipAddressCommand { get; }
    public Command ShipAddressTappedCommand { get; }
    public Command ShipAddressCloseCommand { get; }

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
        if (Items.Count < 18)
            return;
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _purchaseSupplierService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, SearchText.Text, Items.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
					_userDialogs.ShowLoading("Yükleniyor...");
					foreach (var item in result.Data)
                        Items.Add(Mapping.Mapper.Map<PurchaseSupplier>(item));

                   
                }
            }

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
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

    private async Task ItemTappedAsync(PurchaseSupplier item)
    {
        if (item is null || IsBusy)
            return;

        try
        {
            IsBusy = true;

            // Aynı öğeye tıklanmışsa seçimi kaldır
            if (PurchaseSupplier == item)
            {
                item.IsSelected = false;
                PurchaseSupplier = null;
                return;
            }

            // Önceki seçili öğeyi kaldır
            if (PurchaseSupplier != null)
            {
                PurchaseSupplier.IsSelected = false;
            }

            PurchaseSupplier = item;

            if (item.ShipAddressCount > 0)
            {
                await LoadShipAddressesAsync(item);
                CurrentPage.FindByName<BottomSheet>("shipAddressBottomSheet").State = BottomSheetState.HalfExpanded;
            }
            else
            {
                // ShipAddress olmadan seçim yapılabilir
                item.IsSelected = true;
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

            SelectedShipAddressModel = shipAddressModel;
            if (shipAddressModel.IsSelected)
            {
                SelectedShipAddressModel.IsSelected = false;
                SelectedShipAddressModel = null;
            }
            else
            {
                ShipAddresses.ToList().ForEach(x => x.IsSelected = false);
                SelectedShipAddressModel = shipAddressModel;
                SelectedShipAddressModel.IsSelected = true;
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
                PurchaseSupplier.ShipAddressName = selectedShipAddress.Name;

                Items.ToList().ForEach(x => x.IsSelected = false);

                PurchaseSupplier.IsSelected = true;

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

            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
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

            if (PurchaseSupplier is null)
            {
                _userDialogs.Alert("Lütfen bir tedarikçi seçiniz.", "Hata", "Tamam");
                return;
            }

            if (PurchaseSupplier.ShipAddressCount > 0 && PurchaseSupplier.ShipAddressReferenceId == 0)
            {
                await _userDialogs.AlertAsync("Lütfen bir sevk adresi seçiniz.", "Hata", "Tamam");
                return;
            }

            await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseOrderProcessBasketListView)}", new Dictionary<string, object>
            {
                [nameof(WarehouseModel)] = WarehouseModel,
                [nameof(PurchaseSupplier)] = PurchaseSupplier
            });
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

    private async Task BackAsyc()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var confirm = await _userDialogs.ConfirmAsync("Verileriniz silinecektir! Devam etmek istediğinize emin misiniz?", "İptal", "Evet", "Hayır");
            if (!confirm)
                return;

            Items.Clear();
            ShipAddresses.Clear();
            if (PurchaseSupplier is not null)
            {
                PurchaseSupplier.ShipAddressReferenceId = 0;
                PurchaseSupplier.ShipAddressCode = string.Empty;
                PurchaseSupplier.ShipAddressName = string.Empty;
                PurchaseSupplier.IsSelected = false;
            }
            PurchaseSupplier = null;

            Items.ForEach(x => x.IsSelected = false);

            await Shell.Current.GoToAsync("..");
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

    public async Task ClearPageAsync()
    {
        try
        {
            await Task.Run(() =>
            {
                ShipAddresses.Clear();
                if (PurchaseSupplier is not null)
                {
                    PurchaseSupplier.ShipAddressReferenceId = 0;
                    PurchaseSupplier.ShipAddressCode = string.Empty;
                    PurchaseSupplier.ShipAddressName = string.Empty;

                    PurchaseSupplier.IsSelected = false;
                }
                PurchaseSupplier = null;

                Items.ForEach(x => x.IsSelected = false);
            });
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
    }
}