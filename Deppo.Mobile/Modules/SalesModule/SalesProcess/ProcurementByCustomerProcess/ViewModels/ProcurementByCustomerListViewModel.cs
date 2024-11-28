using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProcurementModels.ByCustomerModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.ShipAddressModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.Views;
using DevExpress.Maui.Controls;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class ProcurementByCustomerListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IProcurementByCustomerService _procurementByCustomerService;
    private readonly IShipAddressService _shipAddressService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    WarehouseModel? warehouseModel;

    [ObservableProperty]
    ProcurementCustomerModel selectedCustomerModel;

    [ObservableProperty]
    ShipAddressModel selectedShipAddressModel;

    [ObservableProperty]
    public SearchBar searchText;

    public ProcurementByCustomerListViewModel(
        IHttpClientService httpClientService,
        IProcurementByCustomerService procurementByCustomerService,
        IShipAddressService shipAddressService,
        IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _procurementByCustomerService = procurementByCustomerService;
        _shipAddressService = shipAddressService;
        _userDialogs = userDialogs;

        Title = "Siparişi Olan Müşteriler";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<ProcurementCustomerModel>(async (item) => await ItemTappedAsync(item));
        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
        ShipAddressTappedCommand = new Command<ShipAddressModel>(async (item) => await ShipAddressTappedAsync(item));
        ConfirmShipAddressCommand = new Command(ConfirmShipAddressAsync);
        ShipAddressCloseCommand = new Command(async () => await ShipAddressCloseAsync());
        NextViewCommand = new Command(async () => await NextViewAsync());
        BackCommand = new Command(async () => await BackAsyc());

    }

    public Page CurrentPage { get; set; }

    public ObservableCollection<ProcurementCustomerModel> Items { get; } = new();
    public ObservableCollection<ShipAddressModel> ShipAddresses { get; } = new();

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command<ProcurementCustomerModel> ItemTappedCommand { get; }
    public Command PerformSearchCommand { get; }
    public Command PerformEmptySearchCommand { get; }
    public Command SwipeItemCommand { get; }
    public Command<ShipAddressModel> ShipAddressTappedCommand { get; }
    public Command ConfirmShipAddressCommand { get; }
    public Command ShipAddressCloseCommand { get; }
    public Command NextViewCommand { get; }
    public Command BackCommand { get; }


    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor...");
            Items.Clear();
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _procurementByCustomerService.GetObjects(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                warehouseNumber: WarehouseModel?.Number ?? 0,
                search: SearchText.Text,
                skip: 0,
                take: 20
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                    foreach (var item in result.Data)
                    {
                        var customer = Mapping.Mapper.Map<ProcurementCustomerModel>(item);
                        Items.Add(customer);
                    }
            }

            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
        }
        catch (System.Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert($"{ex.Message}", "Hata", "Tamam");

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
        if (Items.Count < 18)
            return;

        try
        {
            IsBusy = true;
           
            _userDialogs.ShowLoading("Yükleniyor...");
           
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _procurementByCustomerService.GetObjects(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                warehouseNumber: WarehouseModel?.Number ?? 0,
                search: SearchText.Text,
                skip: Items.Count,
                take: 20
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                    foreach (var item in result.Data)
                    {
                        var customer = Mapping.Mapper.Map<ProcurementCustomerModel>(item);
                        Items.Add(customer);
                    }
            }

            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
        }
        catch (System.Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert($"{ex.Message}", "Hata", "Tamam");

        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ItemTappedAsync(ProcurementCustomerModel item)
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
                    SelectedCustomerModel = item;
                    await LoadShipAddressesAsync(item);
                    CurrentPage.FindByName<BottomSheet>("shipAddressBottomSheet").State = BottomSheetState.HalfExpanded;
                }
                else
                {
                    item.IsSelected = true;
                    SelectedCustomerModel = item;
					Items.Where(x => x.Code != item.Code).ToList().ForEach(x => x.IsSelected = false);
				}
            }
            else
            {
                item.IsSelected = false;
                SelectedCustomerModel = null;

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

    private async Task LoadShipAddressesAsync(ProcurementCustomerModel customer)
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

    private void ConfirmShipAddressAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var selectedShipAddress = ShipAddresses.FirstOrDefault(x => x.IsSelected);
            if (selectedShipAddress is not null)
            {
                SelectedCustomerModel.ShipAddressReferenceId = selectedShipAddress.ReferenceId;
                SelectedCustomerModel.ShipAddressCode = selectedShipAddress.Code;
                SelectedCustomerModel.ShipAddressName = selectedShipAddress.Name;

                Items.ToList().ForEach(x => x.IsSelected = false);

                SelectedCustomerModel.IsSelected = true;

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

    private async Task ShipAddressCloseAsync()
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            SelectedCustomerModel = null;
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
            var result = await _procurementByCustomerService.GetObjects(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                warehouseNumber: WarehouseModel.Number,
                skip: 0,
                take: 20,
                search: SearchText.Text);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                    Items.Add(Mapping.Mapper.Map<ProcurementCustomerModel>(item));
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

			SelectedCustomerModel = null;
			SearchText.Text = string.Empty;
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

    private async Task NextViewAsync()
    {
        if (IsBusy)
            return;
        if (SelectedCustomerModel is null)
            return;
        try
        {

            if(SelectedCustomerModel.ShipAddressCount > 0 && string.IsNullOrEmpty(SelectedCustomerModel.ShipAddressCode))
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

                await _userDialogs.AlertAsync("Lütfen seçili müşteriye bir sevk adresi seçiniz", "Uyarı", "Tamam");
                return;
            }

            SearchText.Text = string.Empty;
			await Shell.Current.GoToAsync($"{nameof(ProcurementByCustomerProcurementWarehouseListView)}", new Dictionary<string, object>
			{
				["OrderWarehouseModel"] = WarehouseModel,
				[nameof(ProcurementCustomerModel)] = SelectedCustomerModel
			});
		}
        catch (Exception ex)
        {
            if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
        finally
        {
            IsBusy = false;
        }
        
    }

}
