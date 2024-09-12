using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.VariantModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.Views;
using DevExpress.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(PurchaseSupplier), queryId: nameof(PurchaseSupplier))]
public partial class InputProductPurchaseOrderProcessProductListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly IServiceProvider _serviceProvider;
    private readonly IWaitingPurchaseOrderService _waitingPurchaseOrderService;
    private readonly IPurchaseSupplierProductService _purchaseSupplierProductService;

    [ObservableProperty]
    private ObservableCollection<PurchaseSupplierProduct> selectedProducts = new();

    [ObservableProperty]
    private ObservableCollection<WaitingPurchaseOrderModel> selectedOrders = new();

    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    private PurchaseSupplier purchaseSupplier = null!;

    [ObservableProperty]
    private int targetViewType = default;

    [ObservableProperty]
    private bool isProductVisible = true;

    [ObservableProperty]
    private bool isOrderVisible = false;

    public Page CurrentPage { get; set; }

    public InputProductPurchaseOrderProcessProductListViewModel(
        IHttpClientService httpClientService, IUserDialogs userDialogs,
        IServiceProvider serviceProvider,
        IWaitingPurchaseOrderService waitingPurchaseOrderService,
        IPurchaseSupplierProductService purchaseSupplierProductService)
    {
        _httpClientService = httpClientService;

        _userDialogs = userDialogs;
        _serviceProvider = serviceProvider;
        _waitingPurchaseOrderService = waitingPurchaseOrderService;
        _purchaseSupplierProductService = purchaseSupplierProductService;

        Title = "Ürün Listesi";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<PurchaseSupplierProduct>(async (x) => await ItemTappedAsync(x));

        LoadOrdersCommand = new Command(async () => await LoadOrdersAsync());
        LoadMoreOrdersCommand = new Command(async () => await LoadMoreOrdersAsync());
        OrderTappedCommand = new Command<WaitingPurchaseOrderModel>(async (x) => await OrderTappedAsync(x));

        SwitchViewCommand = new Command(async () => await SwitchViewAsync());
        NextViewCommand = new Command(async () => await NextViewAsync());
    }

    public Command SwitchViewCommand { get; }
    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command<PurchaseSupplierProduct> ItemTappedCommand { get; }

    public Command LoadOrdersCommand { get; }
    public Command LoadMoreOrdersCommand { get; }
    public Command<WaitingPurchaseOrderModel> OrderTappedCommand { get; }

    public Command NextViewCommand { get; }

    public ObservableCollection<WaitingPurchaseOrderModel> Orders { get; } = new();
    public ObservableCollection<PurchaseSupplierProduct> Items { get; } = new();

    private async Task SwitchViewAsync()
    {
        if (IsBusy)
            return;

        try
        {
            switch (TargetViewType)
            {
                case 0:
                    await SwitchToOrderListViewAsync();
                    break;

                case 1:
                    await SwitchToProductListViewAsync();
                    break;

                default:
                    await SwitchToProductListViewAsync();
                    break;
            }
        }
        catch (System.Exception)
        {
            _userDialogs.Alert("Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    //TargetViewType = 0
    private async Task SwitchToProductListViewAsync()
    {
        try
        {
            TargetViewType = 0;
            IsProductVisible = true;
            IsOrderVisible = false;
            CurrentPage.FindByName<Border>("productView").IsVisible = true;
            CurrentPage.FindByName<Border>("orderView").IsVisible = false;
            Orders.Clear();
            Title = "Ürün Listesi";
            SelectedOrders.Clear();
            await LoadItemsAsync();
        }
        catch (System.Exception ex)
        {
            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
    }

    //TargetViewType = 1
    private async Task SwitchToOrderListViewAsync()
    {
        try
        {
            TargetViewType = 1;
            IsProductVisible = false;
            IsOrderVisible = true;
            SelectedProducts.Clear();
            CurrentPage.FindByName<Border>("productView").IsVisible = false;
            CurrentPage.FindByName<Border>("orderView").IsVisible = true;
            Items.Clear();
            Title = "Sipariş Listesi";
            SelectedProducts.Clear();
            await LoadOrdersAsync();
        }
        catch (System.Exception ex)
        {
            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
    }

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
            var result = await _purchaseSupplierProductService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, PurchaseSupplier.ReferenceId, WarehouseModel.Number, string.Empty, 0, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                        Items.Add(Mapping.Mapper.Map<PurchaseSupplierProduct>(item));
                }
            }

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
            var result = await _purchaseSupplierProductService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, PurchaseSupplier.ReferenceId, WarehouseModel.Number, string.Empty, Items.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                        Items.Add(Mapping.Mapper.Map<PurchaseSupplierProduct>(item));

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

    private async Task LoadOrdersAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor...");
            Orders.Clear();
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _waitingPurchaseOrderService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, PurchaseSupplier.ReferenceId, string.Empty, 0, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                        Orders.Add(Mapping.Mapper.Map<WaitingPurchaseOrderModel>(item));
                }
            }

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

    private async Task LoadMoreOrdersAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _waitingPurchaseOrderService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, PurchaseSupplier.ReferenceId, string.Empty, Orders.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                        Orders.Add(Mapping.Mapper.Map<WaitingPurchaseOrderModel>(item));
                }
            }

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

    private async Task ItemTappedAsync(PurchaseSupplierProduct purchaseSupplierProduct)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            var selectedItem = Items.FirstOrDefault(x => x.ItemReferenceId == purchaseSupplierProduct.ItemReferenceId);
            if (selectedItem is not null)
            {
                if (selectedItem.IsSelected)
                {
                    Items.FirstOrDefault(x => x.ItemReferenceId == purchaseSupplierProduct.ItemReferenceId).IsSelected = false;
                    SelectedProducts.Remove(SelectedProducts.FirstOrDefault(x => x.ItemReferenceId == selectedItem.ItemReferenceId));
                }
                else
                {
                    Items.FirstOrDefault(x => x.ItemReferenceId == purchaseSupplierProduct.ItemReferenceId).IsSelected = true;

                    // var basketItem = new InputPurchaseBasketModel
                    // {
                    //     ItemReferenceId = selectedItem.ItemReferenceId,
                    //     ItemCode = selectedItem.ItemCode,
                    //     ItemName = selectedItem.ItemName,
                    //     IsVariant = selectedItem.IsVariant,
                    //     UnitsetReferenceId = selectedItem.UnitsetReferenceId,
                    //     UnitsetCode = selectedItem.UnitsetCode,
                    //     UnitsetName = selectedItem.UnitsetName,
                    //     SubUnitsetReferenceId = selectedItem.SubUnitsetReferenceId,
                    //     SubUnitsetCode = selectedItem.SubUnitsetCode,
                    //     SubUnitsetName = selectedItem.SubUnitsetName,
                    //     MainItemReferenceId = selectedItem.MainItemReferenceId,
                    //     MainItemCode = selectedItem.MainItemCode,
                    //     MainItemName = selectedItem.MainItemName,
                    //     StockQuantity = default,
                    //     IsSelected = false,
                    //     TrackingType = selectedItem.TrackingType,
                    //     LocTracking = selectedItem.LocTracking,
                    //     Image = string.Empty,
                    //     Quantity = selectedItem.WaitingQuantity,

                    // };

                    // if (selectedItem.LocTracking == 1 || selectedItem.TrackingType == 1)
                    //     basketItem.InputQuantity = 0;
                    // else
                    //     basketItem.InputQuantity = 1;

                    SelectedProducts.Add(selectedItem);
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

    private async Task OrderTappedAsync(WaitingPurchaseOrderModel waitingPurchaseOrderModel)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            var selectedItem = Orders.FirstOrDefault(x => x.ReferenceId == waitingPurchaseOrderModel.ReferenceId);
            if (selectedItem is not null)
            {
                if (selectedItem.IsSelected)
                {
                    Orders.FirstOrDefault(x => x.ReferenceId == waitingPurchaseOrderModel.ReferenceId).IsSelected = false;
                    SelectedOrders.Remove(SelectedOrders.FirstOrDefault(x => x.ReferenceId == selectedItem.ReferenceId));
                }
                else
                {
                    Orders.FirstOrDefault(x => x.ReferenceId == waitingPurchaseOrderModel.ReferenceId).IsSelected = true;
                    SelectedOrders.Add(selectedItem);

                    // if (SelectedProducts.ToList().Exists(x => x.ItemReferenceId == selectedItem.ProductReferenceId))
                    // {
                    //     SelectedProducts.FirstOrDefault(x => x.ItemReferenceId == selectedItem.ProductReferenceId).Quantity += selectedItem.WaitingQuantity;
                    // }
                    // else
                    // {
                    // var basketItem = new InputPurchaseBasketModel
                    // {
                    //     ItemReferenceId = selectedItem.IsVariant ? selectedItem.VariantReferenceId : selectedItem.ProductReferenceId,
                    //     ItemCode = selectedItem.IsVariant ? selectedItem.VariantCode : selectedItem.ProductCode,
                    //     ItemName = selectedItem.IsVariant ? selectedItem.VariantName : selectedItem.VariantName,
                    //     IsVariant = selectedItem.IsVariant,
                    //     UnitsetReferenceId = selectedItem.UnitsetReferenceId,
                    //     UnitsetCode = selectedItem.UnitsetCode,
                    //     UnitsetName = selectedItem.UnitsetName,
                    //     SubUnitsetReferenceId = selectedItem.SubUnitsetReferenceId,
                    //     SubUnitsetCode = selectedItem.SubUnitsetCode,
                    //     SubUnitsetName = selectedItem.SubUnitsetName,
                    //     MainItemReferenceId = selectedItem.ProductReferenceId,
                    //     MainItemCode = selectedItem.ProductCode,
                    //     MainItemName = selectedItem.VariantName,
                    //     StockQuantity = default,
                    //     IsSelected = false,
                    //     TrackingType = selectedItem.TrackingType,
                    //     LocTracking = selectedItem.LocTracking,
                    //     Image = string.Empty,
                    //     Quantity = selectedItem.WaitingQuantity,

                    // };

                    // if (selectedItem.LocTracking == 1 || selectedItem.TrackingType == 1)
                    //     basketItem.InputQuantity = 0;
                    // else
                    //     basketItem.InputQuantity = 1;

                    //}
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

    private async Task NextViewAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            if (SelectedOrders.Count == 0 && SelectedProducts.Count == 0)
            {
                await _userDialogs.AlertAsync("Lütfen işlem yapılabilecek bir ürün veya sipariş seçiniz.", "Uyarı", "Tamam");
                return;
            }
            CancellationTokenSource cts = new();
            ObservableCollection<InputPurchaseBasketModel> basketItems = new();
            switch (TargetViewType)
            {
                case 0:
                    basketItems = await ConvertProductItems().WaitAsync(cts.Token);
                    break;

                case 1:
                    basketItems = await ConvertOrderItems().WaitAsync(cts.Token);
                    break;

                default:
                    break;
            }

            await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseOrderProcessBasketListView)}", new Dictionary<string, object>
            {
                [nameof(WarehouseModel)] = WarehouseModel,
                [nameof(PurchaseSupplier)] = PurchaseSupplier,
                ["Items"] = basketItems
            });
        }
        catch (System.Exception ex)
        {
            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task<ObservableCollection<InputPurchaseBasketModel>> ConvertOrderItems()
    {
        return await Task.Run(() =>
        {
            ObservableCollection<InputPurchaseBasketModel> basketItems = new();
            foreach (var item in SelectedOrders)
            {
                var basketItem = new InputPurchaseBasketModel
                {
                    ItemReferenceId = item.ProductReferenceId,
                    ItemCode = item.ProductCode,
                    ItemName = item.ProductName,
                    IsVariant = item.IsVariant,
                    UnitsetReferenceId = item.UnitsetReferenceId,
                    UnitsetCode = item.UnitsetCode,
                    UnitsetName = item.UnitsetName,
                    SubUnitsetReferenceId = item.SubUnitsetReferenceId,
                    SubUnitsetCode = item.SubUnitsetCode,
                    SubUnitsetName = item.SubUnitsetName,
                    MainItemReferenceId = item.ProductReferenceId,
                    MainItemCode = item.ProductCode,
                    MainItemName = item.ProductName,
                    StockQuantity = default,
                    IsSelected = false,
                    TrackingType = item.TrackingType,
                    LocTracking = item.LocTracking,
                    Image = string.Empty,
                    Quantity = item.WaitingQuantity,
                };

                if (item.LocTracking == 1 || item.TrackingType == 1)
                    basketItem.InputQuantity = 0;
                else
                    basketItem.InputQuantity = 1;

                basketItems.Add(basketItem);
            }

            return basketItems;
        });
    }

    private async Task<ObservableCollection<InputPurchaseBasketModel>> ConvertProductItems()
    {
        return await Task.Run(async () =>
        {
            ObservableCollection<InputPurchaseBasketModel> basketItems = new();
            foreach (var item in SelectedProducts)
            {
                var basketItem = new InputPurchaseBasketModel
                {
                    ItemReferenceId = item.ItemReferenceId,
                    ItemCode = item.ItemCode,
                    ItemName = item.ItemName,
                    IsVariant = item.IsVariant,
                    UnitsetReferenceId = item.UnitsetReferenceId,
                    UnitsetCode = item.UnitsetCode,
                    UnitsetName = item.UnitsetName,
                    SubUnitsetReferenceId = item.SubUnitsetReferenceId,
                    SubUnitsetCode = item.SubUnitsetCode,
                    SubUnitsetName = item.SubUnitsetName,
                    MainItemReferenceId = item.MainItemReferenceId,
                    MainItemCode = item.MainItemCode,
                    MainItemName = item.MainItemName,
                    StockQuantity = default,
                    IsSelected = false,
                    TrackingType = item.TrackingType,
                    LocTracking = item.LocTracking,
                    Image = string.Empty,
                    Quantity = item.WaitingQuantity,
                };

                if (item.LocTracking == 1 || item.TrackingType == 1)
                    basketItem.InputQuantity = 0;
                else
                    basketItem.InputQuantity = 1;

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _waitingPurchaseOrderService.GetObjectsByProduct(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, warehouseNumber: WarehouseModel.Number, supplierReferenceId: PurchaseSupplier.ReferenceId, productReferenceId: item.ItemReferenceId, string.Empty, 0, 999999999);
                if (result.IsSuccess)
                {
                    if (result.Data is not null)
                    {
                        foreach (var purchaseOrder in result.Data)
                        {
                            basketItem.Orders.Add(new InputPurchaseBasketOrderModel
                            {
                                OrderReferenceId = purchaseOrder.ReferenceId,
                                SupplierReferenceId = purchaseOrder.SupplierReferenceId,
                                SupplierCode = purchaseOrder.SupplierCode,
                                SupplierName = purchaseOrder.SupplierName,
                                ProductReferenceId = purchaseOrder.ProductReferenceId,
                                ProductCode = purchaseOrder.ProductCode,
                                ProductName = purchaseOrder.ProductName,
                                UnitsetReferenceId = purchaseOrder.UnitsetReferenceId,
                                UnitsetCode = purchaseOrder.UnitsetCode,
                                UnitsetName = purchaseOrder.UnitsetName,
                                SubUnitsetName = purchaseOrder.SubUnitsetName,
                                SubUnitsetCode = purchaseOrder.SubUnitsetCode,
                                SubUnitsetReferenceId = purchaseOrder.UnitsetReferenceId,
                                Quantity = purchaseOrder.Quantity,
                                ShippedQuantity = purchaseOrder.ShippedQuantity,
                                WaitingQuantity = purchaseOrder.WaitingQuantity,
                                OrderDate = purchaseOrder.OrderDate,
                                DueDate = purchaseOrder.DueDate,
                            });
                        }
                    }
                }

                basketItems.Add(basketItem);
            }

            return basketItems;
        });
    }
}