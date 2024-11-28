using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.ProductionInput.Views;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.ProductionInput.ViewModels;

public partial class ProductionInputWarehouseListViewModel : BaseViewModel
{
    private IHttpClientService _httpClientService;
    private IWarehouseService _warehouseService;
    private IUserDialogs _userDialogs;

    [ObservableProperty]
    WarehouseModel selectedWarehouseModel;

    public ProductionInputWarehouseListViewModel(IHttpClientService httpClientService, IWarehouseService warehouseService, IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _warehouseService = warehouseService;
        _userDialogs = userDialogs;

        Title = "Ambar Seçimi";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        ItemTappedCommand = new Command<WarehouseModel>(ItemTappedAsync);
        NextCommand = new Command(async () => await NextAsync());

    }

    //Delegate
    public Command LoadItemsCommand { get; }
    public Command<WarehouseModel> ItemTappedCommand { get; }
    public Command NextCommand { get; }

    public ObservableCollection<WarehouseModel> Items { get; } = new();

    //Event
    public async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("Loading Items...");
            await Task.Delay(1000);
            Items.Clear();

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseService.GetObjects(httpClient, string.Empty, null, 0, 999999999, _httpClientService.FirmNumber);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                    foreach (var item in result.Data)
                        Items.Add(new WarehouseModel
                        {
                            ReferenceId = item.ReferenceId,
                            Number = item.Number,
                            Name = item.Name,
                            City = item.City,
                            Country = item.Country,
                            IsSelected = false

                        });
            }

            _userDialogs.HideHud();
        }
        catch (System.Exception ex)
        {
            _userDialogs.HideHud();
            _userDialogs.Alert(ex.Message, "Hata");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void ItemTappedAsync(WarehouseModel item)
    {
        if (item is null)
            return;

        SelectedWarehouseModel = item;
        Items.ToList().ForEach(x => x.IsSelected = false);
        Items.FirstOrDefault(x => x.ReferenceId == item.ReferenceId).IsSelected = true;
    }

    public async Task NextAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(ProductionInputProductListView)}", new Dictionary<string, object>
        {
            [nameof(WarehouseModel)] = SelectedWarehouseModel
        });
    }
}
