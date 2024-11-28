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
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.Views;
using DevExpress.Maui.Controls;
using DevExpress.Maui.Core.Internal;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class OutputProductSalesOrderProcessCustomerListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ISalesCustomerService _salesCustomerService;
    private readonly ISalesCustomerProductService _salesCustomerProductService;
    private readonly IShipAddressService _shipAddressService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    private SalesCustomer? salesCustomer;

    [ObservableProperty]
    private ShipAddressModel? selectedShipAddressModel;

    #region Collections

    public ObservableCollection<SalesCustomer> Items { get; } = new();

    public ObservableCollection<ShipAddressModel> ShipAddresses { get; } = new();

    #endregion Collections

    public OutputProductSalesOrderProcessCustomerListViewModel(IHttpClientService httpClientService,
    ISalesCustomerService salesCustomerService,
    IShipAddressService shipAddressService,
    IUserDialogs userDialogs,
    ISalesCustomerProductService salesCustomerProductService,
    IServiceProvider serviceProvider)
    {
        _httpClientService = httpClientService;
        _salesCustomerService = salesCustomerService;
        _shipAddressService = shipAddressService;
        _userDialogs = userDialogs;
        _salesCustomerProductService = salesCustomerProductService;
        _serviceProvider = serviceProvider;

        Title = "Müşteriler";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<SalesCustomer>(async (customer) => await ItemTappedAsync(customer));
        NextViewCommand = new Command(async () => await NextViewAsync());
        SwipeItemCommand = new Command<SalesCustomer>(async (customer) => await SwipeItemAsync(customer));

        ShipAddressTappedCommand = new Command<ShipAddressModel>(async (shipAddress) => await ShipAddressTappedAsync(shipAddress));
        ConfirmShipAddressCommand = new Command(async () => await ConfirmShipAddressAsync());
        ShipAddressCloseCommand = new Command(async () => await ShipAddressCloseAsync());

        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
        BackCommand = new Command(async () => await BackAsyc());
    }

    public Page CurrentPage { get; set; } = null!;

    [ObservableProperty]
    public SearchBar searchText;

    #region Commands

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command SwipeItemCommand { get; }
    public Command NextViewCommand { get; }

    public Command ConfirmShipAddressCommand { get; }
    public Command ShipAddressTappedCommand { get; }
    public Command ShipAddressCloseCommand { get; }

    public Command PerformSearchCommand { get; }
    public Command PerformEmptySearchCommand { get; }
    public Command BackCommand { get; }

    #endregion Commands

    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("Loading Items...");
            Items.Clear();
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _salesCustomerService.GetObjectsAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, warehouseNumber: WarehouseModel.Number, skip: 0, take: 20, search: SearchText.Text);
            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                    Items.Add(Mapping.Mapper.Map<SalesCustomer>(item));
            }

            _userDialogs.HideHud();
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message);
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

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _salesCustomerService.GetObjectsAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, warehouseNumber: WarehouseModel.Number, skip: Items.Count, take: 20, search: SearchText.Text);
            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                    Items.Add(Mapping.Mapper.Map<SalesCustomer>(item));
            }

            _userDialogs.HideHud();
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SwipeItemAsync(SalesCustomer selectedItem)
    {
        if (selectedItem is null)
            return;
        if (IsBusy)
            return;
        try
        {
            await ShowOrdersAsync(selectedItem);
            CurrentPage.FindByName<BottomSheet>("customerProductsBottomSheet").State = BottomSheetState.HalfExpanded;
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

    private async Task ShowOrdersAsync(SalesCustomer selectedItem)
    {
        if (selectedItem is null)
            return;
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("Loading Orders...");
            await Task.Delay(1000);

            SalesCustomer = selectedItem;

            if (selectedItem?.Products?.Count > 0)
            {
                selectedItem.Products.Clear();
            }

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var customerOrders = await _salesCustomerProductService.GetObjects(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                customerReferenceId: selectedItem.ReferenceId,
                warehouseNumber: WarehouseModel.Number,
                skip: 0,
                take: 9999999
            );

            if (customerOrders.IsSuccess)
            {
                if (customerOrders.Data is null)
                    return;

                foreach (var item in customerOrders.Data)
                {
                    var customer = Items.FirstOrDefault(x => x.ReferenceId == selectedItem.ReferenceId);
                    if (customer?.Products is null)
                    {
                        customer.Products = new();
                    }
                    if (customer is not null)
                    {
                        customer.Products.Add(Mapping.Mapper.Map<SalesCustomerProduct>(item));
                    }
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

    private async Task ItemTappedAsync(SalesCustomer item)
    {
        if (item is null)
            return;
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (!item.IsSelected)
            {
                if (item.ShipAddressCount > 0)
                {
                    SalesCustomer = item;
                    await LoadShipAddressesAsync(item);
                    CurrentPage.FindByName<BottomSheet>("shipAddressBottomSheet").State = BottomSheetState.HalfExpanded;
                }
                else
                {
                    item.IsSelected = true;
                    SalesCustomer = item;
                }
            }
            else
            {
                item.IsSelected = false;
                SalesCustomer = null;
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

    private async Task LoadShipAddressesAsync(SalesCustomer customer)
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
                currentReferenceId: customer.ReferenceId,
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
                SalesCustomer.ShipAddressReferenceId = selectedShipAddress.ReferenceId;
                SalesCustomer.ShipAddressCode = selectedShipAddress.Code;
                SalesCustomer.ShipAddressName = selectedShipAddress.Name;

                Items.ToList().ForEach(x => x.IsSelected = false);

                SalesCustomer.IsSelected = true;

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

            // Müşteri kontrolü
            if (SalesCustomer is null)
            {
                await _userDialogs.AlertAsync("Lütfen bir müşteri seçiniz.", "Hata", "Tamam");
                return;
            }

            // Sevk adresi seçilmiş mi kontrolü
            if (SalesCustomer.ShipAddressCount > 0 && SalesCustomer.ShipAddressReferenceId == 0)
            {
                await _userDialogs.AlertAsync("Lütfen bir sevk adresi seçiniz.", "Hata", "Tamam");
                return;
            }

            await Shell.Current.GoToAsync($"{nameof(OutputProductSalesOrderProcessBasketListView)}", new Dictionary<string, object>
            {
                [nameof(SalesCustomer)] = SalesCustomer,
                [nameof(WarehouseModel)] = WarehouseModel,
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

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            Items.Clear();
            var result = await _salesCustomerService.GetObjectsAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, warehouseNumber: WarehouseModel.Number, skip: 0, take: 20, search: SearchText.Text);
            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                    Items.Add(Mapping.Mapper.Map<SalesCustomer>(item));
            }
            else
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
            if (SalesCustomer is not null)
            {
                SalesCustomer.ShipAddressReferenceId = 0;
                SalesCustomer.ShipAddressCode = string.Empty;
                SalesCustomer.ShipAddressName = string.Empty;
                SalesCustomer.IsSelected = false;
            }
            SalesCustomer = null;
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
                if (SalesCustomer is not null)
                {
                    SalesCustomer.ShipAddressReferenceId = 0;
                    SalesCustomer.ShipAddressCode = string.Empty;
                    SalesCustomer.ShipAddressName = string.Empty;

                    SalesCustomer.IsSelected = false;
                }
                SalesCustomer = null;

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