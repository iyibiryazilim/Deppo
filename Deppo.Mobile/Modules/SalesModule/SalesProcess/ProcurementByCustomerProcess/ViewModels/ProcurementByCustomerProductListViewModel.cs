using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Core.Models.ProcurementModels.ByCustomerModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.Views;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(ProcurementCustomerModel), queryId: nameof(ProcurementCustomerModel))]
[QueryProperty(name: nameof(OrderWarehouseModel), queryId: nameof(OrderWarehouseModel))]
public partial class ProcurementByCustomerProductListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IProcurementByCustomerProductService _procurementByCustomerProductService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    WarehouseModel? warehouseModel;

    [ObservableProperty]
    WarehouseModel? orderWarehouseModel;

    [ObservableProperty]
    ProcurementCustomerModel procurementCustomerModel;

    [ObservableProperty]
    public SearchBar searchText;

    public ProcurementByCustomerProductListViewModel(IHttpClientService httpClientService, IProcurementByCustomerProductService procurementByCustomerProductService, IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _procurementByCustomerProductService = procurementByCustomerProductService;
        _userDialogs = userDialogs;

        Title = "Toplanabilir Ürünler";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<ProcurementCustomerProductModel>(async (item) => await ItemTappedAsync(item));
        NextViewCommand = new Command(async () => await NextViewAsync());
    }

    public Page CurrentPage { get; set; }

    public ObservableCollection<ProcurementCustomerProductModel> Items { get; } = new();

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command<ProcurementCustomerProductModel> ItemTappedCommand { get; }
    public Command SelectAllCommand { get; }
    public Command DeselectAllCommand { get; }
    public Command PerformSearchCommand { get; }
    public Command PerformEmptySearchCommand { get; }
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
            await Task.Delay(1000);
            Items.Clear();

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _procurementByCustomerProductService.GetObjects(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                warehouseNumber: WarehouseModel?.Number ?? 0,
                orderWarehouseNumber: OrderWarehouseModel?.Number ?? 0,
                customerReferenceId: ProcurementCustomerModel.ReferenceId,
                search: string.Empty,
                skip: 0,
                take: 20
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                    foreach (var item in result.Data)
                    {
                        var customer = Mapping.Mapper.Map<ProcurementCustomerProductModel>(item);
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

        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _procurementByCustomerProductService.GetObjects(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                warehouseNumber: WarehouseModel?.Number ?? 0,
                orderWarehouseNumber: OrderWarehouseModel?.Number ?? 0,
                customerReferenceId: ProcurementCustomerModel.ReferenceId,
                search: string.Empty,
                skip: Items.Count,
                take: 20
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                    foreach (var item in result.Data)
                    {
                        var customer = Mapping.Mapper.Map<ProcurementCustomerProductModel>(item);
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

    private async Task ItemTappedAsync(ProcurementCustomerProductModel item)
    {
        // if (IsBusy)
        //     return;

        // try
        // {
        //     IsBusy = true;

        //     if (item is not null)
        //     {
        //         await Task.Run(() =>
        //         {
        //             if (SelectedItems.Contains(item))
        //             {
        //                 item.IsSelected = false;
        //                 SelectedItems.Remove(item);
        //             }
        //             else
        //             {
        //                 item.IsSelected = true;
        //                 SelectedItems.Add(item);
        //             }

        //         });

        //     }

        // }
        // catch (System.Exception ex)
        // {
        //     _userDialogs.Alert($"{ex.Message}", "Hata", "Tamam");
        // }
        // finally
        // {
        //     IsBusy = false;
        // }
    }

    public async Task NextViewAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (Items.Count > 0)
            {

                var param = GetProcurementProductList(Items);

                await Shell.Current.GoToAsync($"{nameof(ProcurementByCustomerBasketView)}", new Dictionary<string, object>
                {
                    [nameof(ProcurementCustomerBasketModel)] = param
                });
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

    /// <summary>
    /// Kısmi veya tamamı toplanabilir ürünleri getirir.
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    private ProcurementCustomerBasketModel GetProcurementProductList(ObservableCollection<ProcurementCustomerProductModel> items)
    {
        var procurementProductList = Items.Where(x => x.StockQuantity >= x.WaitingQuantity || x.StockQuantity > 0).ToList();

        ProcurementCustomerBasketModel procurementCustomerBasketModel = new();
        procurementCustomerBasketModel.ProcurementProductList = procurementProductList;
        procurementCustomerBasketModel.WarehouseNumber = WarehouseModel?.Number ?? 0;
        procurementCustomerBasketModel.WarehouseName = WarehouseModel?.Name ?? string.Empty;

        return procurementCustomerBasketModel;
    }

}
