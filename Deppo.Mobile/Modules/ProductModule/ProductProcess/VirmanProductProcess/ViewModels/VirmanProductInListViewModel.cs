using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.VirmanModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.VirmanProductProcess.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.VirmanProductProcess.ViewModels;

[QueryProperty(name: nameof(OutWarehouse), queryId: nameof(OutWarehouse))]
[QueryProperty(name: nameof(InWarehouse), queryId: nameof(InWarehouse))]
[QueryProperty(name: nameof(WarehouseTotalModel), queryId: nameof(WarehouseTotalModel))]
public partial class VirmanProductInListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ISeriLotTransactionService _serilotTransactionService;
    private readonly IUserDialogs _userDialogs;
    private readonly IProductService _productService;

    [ObservableProperty]
    private WarehouseModel outWarehouse = null!;

    [ObservableProperty]
    private WarehouseModel inWarehouse = null!;

    [ObservableProperty]
    private WarehouseTotalModel warehouseTotalModel = null!;

    public ObservableCollection<ProductModel> Items { get; } = new();

    [ObservableProperty]
    private ProductModel? selectedProduct;

    public Page CurrentPage { get; set; }

    public VirmanProductInListViewModel(IHttpClientService httpClientService, ISeriLotTransactionService serilotTransactionService, IUserDialogs userDialogs, IProductService productService)
    {
        _httpClientService = httpClientService;
        _serilotTransactionService = serilotTransactionService;
        _userDialogs = userDialogs;
        Title = "Giriş Ürünleri Listesi";

        _productService = productService;

        BackCommand = new Command(async () => await BackAsync());
        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<ProductModel>(async (parameter) => await ItemTappedAsync(parameter));
        NextViewCommand = new Command(async () => await NextViewAsync());
    }

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command ConfirmCommand { get; }
    public Command BackCommand { get; }
    public Command NextViewCommand { get; }

    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            Items.Clear();
            _userDialogs.Loading("Yükleniyor...");
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _productService.GetObjects(httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, skip: 0, take: 20);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var product in result.Data)
                {
                    var item = Mapping.Mapper.Map<ProductModel>(product);

                    Items.Add(new ProductModel
                    {
                        ReferenceId = item.ReferenceId,
                        Code = item.Code,
                        Name = item.Name,
                        UnitsetReferenceId = item.UnitsetReferenceId,
                        UnitsetCode = item.UnitsetCode,
                        UnitsetName = item.UnitsetName,
                        SubUnitsetReferenceId = item.SubUnitsetReferenceId,
                        SubUnitsetCode = item.SubUnitsetCode,
                        SubUnitsetName = item.SubUnitsetName,
                        StockQuantity = item.StockQuantity,
                        LocTracking = item.LocTracking,
                        IsVariant = item.IsVariant,
                        TrackingType = item.TrackingType,
                        IsSelected = false,
                        LocTrackingIcon = product.LocTrackingIcon,
                        VariantIcon = product.VariantIcon,
                        TrackingTypeIcon = product.TrackingTypeIcon,
                    });
                }
            }

            _userDialogs.Loading().Hide();
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

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
            _userDialogs.Loading("Yükleniyor...");
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _productService.GetObjects(httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, skip: Items.Count, take: 20);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var product in result.Data)
                {
                    var item = Mapping.Mapper.Map<ProductModel>(product);

                    Items.Add(new ProductModel
                    {
                        ReferenceId = item.ReferenceId,
                        Code = item.Code,
                        Name = item.Name,
                        UnitsetReferenceId = item.UnitsetReferenceId,
                        UnitsetCode = item.UnitsetCode,
                        UnitsetName = item.UnitsetName,
                        SubUnitsetReferenceId = item.SubUnitsetReferenceId,
                        SubUnitsetCode = item.SubUnitsetCode,
                        SubUnitsetName = item.SubUnitsetName,
                        StockQuantity = item.StockQuantity,
                        LocTracking = item.LocTracking,
                        IsVariant = item.IsVariant,
                        TrackingType = item.TrackingType,
                        IsSelected = false,
                        LocTrackingIcon = product.LocTrackingIcon,
                        VariantIcon = product.VariantIcon,
                        TrackingTypeIcon = product.TrackingTypeIcon,
                    });
                }
            }

            _userDialogs.Loading().Hide();
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ItemTappedAsync(ProductModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (SelectedProduct == item)
            {
                SelectedProduct.IsSelected = false;
                SelectedProduct = null;
            }
            else
            {
                if (SelectedProduct is not null)
                {
                    SelectedProduct.IsSelected = false;
                }
                SelectedProduct = item;
                SelectedProduct.IsSelected = true;
            }
        }
        catch (Exception ex)
        {
            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task BackAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            var confirm = await _userDialogs.ConfirmAsync("İşlemi iptal etmek istediğinize emin misiniz?", "İptal", "Evet", "Hayır");
            if (!confirm)
                return;
            if (SelectedProduct != null)
            {
                SelectedProduct.IsSelected = false;
                SelectedProduct = null;
            }

            await Shell.Current.GoToAsync($"..");
        }
        catch (Exception ex)
        {
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

            VirmanBasketModel virmanBasket = new();
            virmanBasket.OutVirmanWarehouse = OutWarehouse;
            virmanBasket.InVirmanWarehouse = InWarehouse;

            InVirmanProductModel ınVirmanProductModel = new();
            ınVirmanProductModel.ReferenceId = (int)SelectedProduct?.ReferenceId;
            ınVirmanProductModel.Code = SelectedProduct?.Code;
            ınVirmanProductModel.Name = SelectedProduct?.Name;
            ınVirmanProductModel.UnitsetReferenceId = (int)SelectedProduct?.UnitsetReferenceId;
            ınVirmanProductModel.UnitsetCode = SelectedProduct?.UnitsetCode;
            ınVirmanProductModel.UnitsetName = SelectedProduct?.UnitsetName;
            ınVirmanProductModel.SubUnitsetReferenceId = (int)SelectedProduct?.SubUnitsetReferenceId;
            ınVirmanProductModel.SubUnitsetCode = SelectedProduct?.SubUnitsetCode;
            ınVirmanProductModel.SubUnitsetName = SelectedProduct?.SubUnitsetName;
            ınVirmanProductModel.StockQuantity = (int)SelectedProduct?.StockQuantity;
            ınVirmanProductModel.LocTracking = SelectedProduct.LocTracking;
            ınVirmanProductModel.IsVariant = SelectedProduct.IsVariant;
            ınVirmanProductModel.TrackingType = SelectedProduct.TrackingType;
            ınVirmanProductModel.IsSelected = false;
            ınVirmanProductModel.LocTrackingIcon = SelectedProduct?.LocTrackingIcon;
            ınVirmanProductModel.VariantIcon = SelectedProduct?.VariantIcon;
            ınVirmanProductModel.TrackingTypeIcon = SelectedProduct?.TrackingTypeIcon;

            virmanBasket.InVirmanProduct = ınVirmanProductModel;

            OutVirmanProductModel outVirmanProductModel = new OutVirmanProductModel();
            outVirmanProductModel.ReferenceId = warehouseTotalModel.ProductReferenceId;
            outVirmanProductModel.Code = warehouseTotalModel.ProductCode;
            outVirmanProductModel.Name = warehouseTotalModel.ProductName;
            outVirmanProductModel.UnitsetReferenceId = warehouseTotalModel.UnitsetReferenceId;
            outVirmanProductModel.UnitsetCode = warehouseTotalModel.UnitsetCode;
            outVirmanProductModel.UnitsetName = warehouseTotalModel.UnitsetName;
            outVirmanProductModel.SubUnitsetReferenceId = warehouseTotalModel.SubUnitsetReferenceId;
            outVirmanProductModel.SubUnitsetCode = warehouseTotalModel.SubUnitsetCode;
            outVirmanProductModel.SubUnitsetName = warehouseTotalModel.SubUnitsetName;
            outVirmanProductModel.StockQuantity = warehouseTotalModel.StockQuantity;
            outVirmanProductModel.LocTracking = warehouseTotalModel.LocTracking;
            outVirmanProductModel.IsVariant = warehouseTotalModel.IsVariant;
            outVirmanProductModel.TrackingType = warehouseTotalModel.TrackingType;
            outVirmanProductModel.IsSelected = false;
            outVirmanProductModel.LocTrackingIcon = warehouseTotalModel.LocTrackingIcon;
            outVirmanProductModel.VariantIcon = warehouseTotalModel.VariantIcon;
            outVirmanProductModel.TrackingTypeIcon = warehouseTotalModel.TrackingTypeIcon;

            virmanBasket.OutVirmanProduct = outVirmanProductModel;

            await Shell.Current.GoToAsync($"{nameof(VirmanProductBasketListView)}", new Dictionary<string, object>
            {
                [nameof(VirmanBasketModel)] = virmanBasket
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
}