using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByLocationProcess.Views;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByLocationProcess.ViewModels;

public partial class ProcurementByLocationWarehouseListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IWarehouseService _warehouseService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    private WarehouseModel? selectedWarehouseModel;

    public ProcurementByLocationWarehouseListViewModel(
        IHttpClientService httpClientService,
        IWarehouseService warehouseService,
        IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _warehouseService = warehouseService;
        _userDialogs = userDialogs;

        Title = "Ambar Seçiniz";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<WarehouseModel>(ItemTappedAsync);
        NextViewCommand = new Command(async () => await NextViewAsync());
    }

    public ObservableCollection<WarehouseModel> Items { get; } = new();

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command<WarehouseModel> ItemTappedCommand { get; }
    public Command NextViewCommand { get; }

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
            var result = await _warehouseService.GetObjectsAsync(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                search: string.Empty,
                skip: 0,
                take: 20
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                    foreach (var item in result.Data)
                    {
                        var warehouse = Mapping.Mapper.Map<WarehouseModel>(item);
                        Items.Add(warehouse);
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

        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor...");

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseService.GetObjectsAsync(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                search: string.Empty,
                skip: Items.Count,
                take: 20
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                    foreach (var item in result.Data)
                    {
                        var warehouse = Mapping.Mapper.Map<WarehouseModel>(item);
                        Items.Add(warehouse);
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

    private void ItemTappedAsync(WarehouseModel warehouse)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (warehouse == SelectedWarehouseModel)
            {
                SelectedWarehouseModel.IsSelected = false;
                SelectedWarehouseModel = null;
            }
            else
            {
                if (SelectedWarehouseModel != null)
                {
                    SelectedWarehouseModel.IsSelected = false;
                }

                SelectedWarehouseModel = warehouse;
                SelectedWarehouseModel.IsSelected = true;
            }

        }
        catch (System.Exception ex)
        {
            _userDialogs.Alert($"{ex.Message}", "Hata", "Tamam");
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

            if (SelectedWarehouseModel is not null)
            {
                if (SelectedWarehouseModel.LocationCount > 0)
                    await Shell.Current.GoToAsync($"{nameof(ProcurementByLocationWarehouseLocationListView)}", new Dictionary<string, object>
                    {
                        [nameof(WarehouseModel)] = SelectedWarehouseModel
                    });
                else
                    _userDialogs.Alert("Seçilen ambarın raf bilgileri bulunmamaktadır.", "Hata", "Tamam");


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
}
