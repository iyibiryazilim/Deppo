using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(PurchaseSupplier), queryId: nameof(PurchaseSupplier))]
public partial class InputProductPurchaseOrderProcessOtherProductListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly IServiceProvider _serviceProvider;
    private readonly IPurchaseSupplierProductService _purchaseSupplierProductService;

    [ObservableProperty]
    private ObservableCollection<PurchaseSupplierProduct> selectedProducts = new();

    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    private PurchaseSupplier purchaseSupplier = null!;

    [ObservableProperty]
    private int targetViewType = default;

    public Page CurrentPage { get; set; }

    public InputProductPurchaseOrderProcessOtherProductListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs,
        IServiceProvider serviceProvider,
        IPurchaseSupplierProductService purchaseSupplierProductService)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _serviceProvider = serviceProvider;
        _purchaseSupplierProductService = purchaseSupplierProductService;

        Title = "Ürün Listesi";
    }

    public Command LoadItemsCommand { get; }

    public Command LoadMoreItemsCommand { get; }

    public Command<PurchaseSupplierProduct> ItemTappedCommand { get; }

    public Command NextViewCommand { get; }

    public ObservableCollection<PurchaseSupplierProduct> Items { get; } = new();

    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor");
            Items.Clear();

            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _purchaseSupplierProductService.GetObjects(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, PurchaseSupplier.ReferenceId, WarehouseModel.Number, string.Empty, 0, 20);
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
}