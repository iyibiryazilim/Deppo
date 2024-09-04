using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.Views;
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

    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    private PurchaseSupplier purchaseSupplier = null!;

    public InputProductPurchaseOrderProcessSupplierListViewModel(IHttpClientService httpClientService,

    IUserDialogs userDialogs,
    IPurchaseSupplierService purchaseSupplierService)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _purchaseSupplierService = purchaseSupplierService;

        Title = "Tedarikçiler";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<PurchaseSupplier>(async (x) => await ItemTappedAsync(x));

        NextViewCommand = new Command(async () => await NextViewAsync());
    }

    public ObservableCollection<PurchaseSupplier> Items { get; } = new();


    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command<SearchBar> PerformSearchCommand { get; }
    public Command ItemTappedCommand { get; }
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
            var result = await _purchaseSupplierService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber,WarehouseModel.Number, string.Empty, 0, 20);
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
            var result = await _purchaseSupplierService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber,WarehouseModel.Number, string.Empty, Items.Count, 20);
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
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            Items.ToList().ForEach(x => x.IsSelected = false);

            var selectedItem = Items.FirstOrDefault(x => x.ReferenceId == purchaseSupplier.ReferenceId);
            if (selectedItem is not null)
            {
                selectedItem.IsSelected = true;
                PurchaseSupplier = selectedItem;
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

    private async Task NextViewAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseOrderProcessProductListView)}", new Dictionary<string, object>
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

}