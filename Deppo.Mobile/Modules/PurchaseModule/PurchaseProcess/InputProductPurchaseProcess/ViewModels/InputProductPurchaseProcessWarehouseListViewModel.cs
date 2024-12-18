using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;

public partial class InputProductPurchaseProcessWarehouseListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IWarehouseService _warehouseService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    private WarehouseModel selectedWarehouseModel = null!;

    public InputProductPurchaseProcessWarehouseListViewModel(IHttpClientService httpClientService,
        IWarehouseService warehouseService,
        IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _warehouseService = warehouseService;
        _userDialogs = userDialogs;

        Title = "Ambar Seçimi";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		ItemTappedCommand = new Command<WarehouseModel>(async (x) => await ItemTappedAsync(x));
		NextViewCommand = new Command(async () => await NextViewAsync());
    }

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command NextViewCommand { get; }

    public ObservableCollection<WarehouseModel> Items { get; } = new();

    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("Loading...");
            Items.Clear();
            await Task.Delay(1000);
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseService.GetObjectsAsync(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                search: string.Empty,
				skip: 0,
                take: 20,
                externalDb: _httpClientService.ExternalDatabase
			);

            if(result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                        Items.Add(Mapping.Mapper.Map<WarehouseModel>(item));
                }
            }

            if(_userDialogs.IsHudShowing)
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

        if (Items.Count < 18)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("Loading...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _warehouseService.GetObjectsAsync(
				 httpClient: httpClient,
				 firmNumber: _httpClientService.FirmNumber,
				 periodNumber: _httpClientService.PeriodNumber,
				 search: string.Empty,
				 skip: Items.Count,
				 take: 20,
				 externalDb: _httpClientService.ExternalDatabase
			);

			if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
					foreach (var item in result.Data)
						Items.Add(Mapping.Mapper.Map<WarehouseModel>(item));
				}
            }

			if (_userDialogs.IsHudShowing)
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

    private async Task ItemTappedAsync(WarehouseModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if(SelectedWarehouseModel == item)
            {
                SelectedWarehouseModel.IsSelected = false;
                SelectedWarehouseModel = null;
			}
            else
            {
                if(SelectedWarehouseModel is not null)
                {
                    SelectedWarehouseModel.IsSelected = false;
                }
                SelectedWarehouseModel = item;
				SelectedWarehouseModel.IsSelected = true;
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

            if (SelectedWarehouseModel is not null)
            {
                await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseProcessSupplierListView)}", new Dictionary<string, object>
                {
                    [nameof(WarehouseModel)] = SelectedWarehouseModel,
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
}