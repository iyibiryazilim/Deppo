using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Helpers.CompanyHelper;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductMenu.Views;
using Deppo.Mobile.Modules.ProductModule.WarehouseMenu.Views;

namespace Deppo.Mobile.Modules.ProductModule.WarehouseMenu.ViewModels;

public partial class WarehouseListViewModel : BaseViewModel
{
    private IHttpClientService _httpClientService;
    private readonly IWarehouseService _warehouseService;
    private readonly IUserDialogs _userDialogs;

    public ObservableCollection<Warehouse> Items { get; } = new();

    [ObservableProperty]
    public SearchBar searchText;

    public WarehouseListViewModel(IWarehouseService warehouseService, IUserDialogs userDialogs, IHttpClientService httpClientService)
    {
        _warehouseService = warehouseService;
        _userDialogs = userDialogs;
        _httpClientService = httpClientService;

        Title = "Ambar Listesi";
        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());

        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
        ItemTappedCommand = new Command<Warehouse>(async (warehouse) => await ItemTappedAsync(warehouse));
    }

    #region Commands

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command PerformSearchCommand { get; }
    public Command PerformEmptySearchCommand { get; }
    public Command RefreshPageCommand { get; }
    public Command ItemTappedCommand { get; }

    #endregion Commands

    public async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            Items.Clear();
            _userDialogs.Loading("Loading Items...");
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var result = await _warehouseService.GetObjectsAsync(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
				search: SearchText.Text,
				skip: 0,
				take: 20,
				externalDb: _httpClientService.ExternalDatabase
            );
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                    Items.Add(Mapping.Mapper.Map<Warehouse>(item));

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
        if (Items.Count < 18)  // 18 equals to PageSize (20) - RemainingItemsThreshold (2)
            return;

        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseService.GetObjectsAsync(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				search: SearchText.Text,
				skip: Items.Count,
				take: 20,
				externalDb: _httpClientService.ExternalDatabase
			);
			if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;
                _userDialogs.Loading("Load more Items...");
                foreach (var item in result.Data)
					Items.Add(Mapping.Mapper.Map<Warehouse>(item));

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

            var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _warehouseService.GetObjectsAsync(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				search: SearchText.Text,
				skip: 0,
				take: 20,
				externalDb: _httpClientService.ExternalDatabase
			);
			if (!result.IsSuccess)
            {
                _userDialogs.Alert(result.Message, "Hata");
                return;
            }

            Items.Clear();
            foreach (var item in result.Data)
				Items.Add(Mapping.Mapper.Map<Warehouse>(item));
		}
        catch (Exception ex)
        {
            _userDialogs.Alert(message: ex.Message, title: "Hata");
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

    private async Task ItemTappedAsync(Warehouse warehouse)
    {
        if (warehouse is null)
            return;
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            WarehouseDetailModel warehouseDetailModel = new();
            warehouseDetailModel.Warehouse = warehouse;

            await Shell.Current.GoToAsync($"{nameof(WarehouseDetailView)}", new Dictionary<string, object>
            {
                [nameof(WarehouseDetailModel)] = warehouseDetailModel
            });
        }
        catch (Exception ex)
        {
            _userDialogs.Alert(ex.Message, "Hata");
        }
        finally
        {
            IsBusy = false;
        }
    }
}