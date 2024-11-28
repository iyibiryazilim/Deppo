using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.ShipAddressModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.Views;
using DevExpress.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class InputProductPurchaseProcessSupplierListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ISupplierService _supplierService;
    private readonly IUserDialogs _userDialogs;
    private readonly IShipAddressService _shipAddressService;

    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    private SupplierModel supplierModel = null!;

    [ObservableProperty]
    private SupplierModel selectedSupplier = null!;

    [ObservableProperty]
    private ShipAddressModel selectedShipAddressModel;

    public Page CurrentPage { get; set; }

    [ObservableProperty]
    public SearchBar searchText;

    public InputProductPurchaseProcessSupplierListViewModel(IHttpClientService httpClientService,
        ISupplierService supplierService,
        IUserDialogs userDialogs, IShipAddressService shipAddressService)
    {
        _httpClientService = httpClientService;
        _supplierService = supplierService;
        _userDialogs = userDialogs;
        _shipAddressService = shipAddressService;

        Title = "Tedarikçiler";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());

        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());

        ItemTappedCommand = new Command<SupplierModel>(async (supplier) => await ItemTappedAsync(supplier));

        ShipAddressTappedCommand = new Command<ShipAddressModel>(async (shipAddress) => await ShipAddressTappedAsync(shipAddress));
        ConfirmShipAddressCommand = new Command(async () => await ConfirmShipAddressAsync());
        ShipAddressCloseCommand = new Command(async () => await ShipAddressCloseAsync());

        NextViewCommand = new Command(async () => await NextViewAsync());
    }

    public ObservableCollection<SupplierModel> Items { get; } = new();
    public ObservableCollection<ShipAddressModel> ShipAddresses { get; } = new();

    // public ObservableCollection<SupplierModel> SelectedItems { get; } = new();
    public Command LoadItemsCommand { get; }

    public Command LoadMoreItemsCommand { get; }
    public Command<SupplierModel> ItemTappedCommand { get; }

    public Command PerformSearchCommand { get; }
    public Command PerformEmptySearchCommand { get; }

    public Command ConfirmShipAddressCommand { get; }
    public Command ShipAddressTappedCommand { get; }
    public Command ShipAddressCloseCommand { get; }

    public Command BackCommand { get; }

    public Command NextViewCommand { get; }

    public async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            Items.Clear();

            _userDialogs.Loading("Loading Items...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            await Task.Delay(1000);
            var result = await _supplierService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SearchText.Text, 0, 20);
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                    Items.Add(Mapping.Mapper.Map<SupplierModel>(item));

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

    public async Task LoadMoreItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            _userDialogs.Loading("Refreshing Items...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _supplierService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SearchText.Text, Items.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                    Items.Add(Mapping.Mapper.Map<SupplierModel>(item));

                if (_userDialogs.IsHudShowing)
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
            var result = await _supplierService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SearchText.Text, Items.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                    Items.Add(Mapping.Mapper.Map<SupplierModel>(item));

                _userDialogs.Loading().Hide();
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

    private async Task ItemTappedAsync(SupplierModel supplier)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            // Eğer seçili değilse
            if (!supplier.IsSelected)
            {
                // Diğer tüm tedarikçilerin IsSelected alanını false yap
                Items.ToList().ForEach(x => x.IsSelected = false);

                // Eğer sevk adresi varsa, sevk adreslerini yükle ve bottom sheet'i aç
                if (supplier.ShipAddressCount > 0)
                {
                    SelectedSupplier = supplier;
                    await LoadShipAddressesAsync(supplier);
                    CurrentPage.FindByName<BottomSheet>("shipAddressBottomSheet").State = BottomSheetState.HalfExpanded;
                }
                else
                {
                    // Sevk adresi yoksa sadece seçili hale getir
                    supplier.IsSelected = true;
                    SelectedSupplier = supplier;
                }
            }
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

    private async Task NextViewAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (WarehouseModel is not null)
            {
                await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseProcessBasketListView)}", new Dictionary<string, object>
                {
                    [nameof(WarehouseModel)] = WarehouseModel,

                    [nameof(SupplierModel)] = SelectedSupplier,

                    [nameof(ShipAddressModel)] = SelectedShipAddressModel,
                });
            }
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

    //shipAddresses
    private async Task LoadShipAddressesAsync(SupplierModel supplierModel)
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
                currentReferenceId: supplierModel.ReferenceId,
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
                //PurchaseSupplier.ShipAddressReferenceId = selectedShipAddress.ReferenceId;
                //PurchaseSupplier.ShipAddressCode = selectedShipAddress.Code;
                //PurchaseSupplier.ShipAddressName = selectedShipAddress.Name;

                Items.ToList().ForEach(x => x.IsSelected = false);

                SelectedSupplier.IsSelected = true;

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

    private async Task ShipAddressCloseAsync()
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            CurrentPage.FindByName<BottomSheet>("shipAddressBottomSheet").State = BottomSheetState.Hidden;
        });
    }
}