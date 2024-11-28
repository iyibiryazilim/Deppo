using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;
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
public partial class InputProductPurchaseOrderProcessOtherProductListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly IServiceProvider _serviceProvider;
    private readonly IPurchaseSupplierProductService _purchaseSupplierProductService;
    private readonly IProductService _productService;

    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    private ProductModel? selectedProduct;

    [ObservableProperty]
    private ObservableCollection<InputPurchaseBasketModel> selectedProducts = new();

    [ObservableProperty]
    private int targetViewType = default;

    public Page CurrentPage { get; set; }

    public InputProductPurchaseOrderProcessOtherProductListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs,
        IServiceProvider serviceProvider,
        IPurchaseSupplierProductService purchaseSupplierProductService, IProductService productService)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _serviceProvider = serviceProvider;
        _purchaseSupplierProductService = purchaseSupplierProductService;
        _productService = productService;

        Title = "Ürün Listesi";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<ProductModel>(async (x) => await ItemTappedAsync(x));
        ConfirmCommand = new Command(async () => await ConfirmAsync());
    }

    public Command LoadItemsCommand { get; }

    public Command LoadMoreItemsCommand { get; }

    public Command NextViewCommand { get; }

    public Command ItemTappedCommand { get; }
    public Command ConfirmCommand { get; }

    public ObservableCollection<ProductModel> Items { get; } = new();

    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            Items.Clear();
            SelectedProducts.Clear();

            _userDialogs.Loading("Loading Items...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            await Task.Delay(1000);
            var result = await _productService.GetObjectsPurchaseProduct(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, string.Empty, 0, 20);
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var product in result.Data)
                {
                    var item = Mapping.Mapper.Map<Product>(product);

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
                        TrackingType = item.TrackingType,
                        LocTracking = item.LocTracking,
                        IsVariant = item.IsVariant,
                        IsSelected = false
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
            _userDialogs.Loading().Dispose();
        }
    }

    private async Task LoadMoreItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.Loading("Loading More Items...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _productService.GetObjectsPurchaseProduct(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, string.Empty, Items.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var product in result.Data)
                {
                    var item = Mapping.Mapper.Map<Product>(product);

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
                        TrackingType = item.TrackingType,
                        LocTracking = item.LocTracking,
                        IsVariant = item.IsVariant,
                        IsSelected = false
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
            _userDialogs.Loading().Dispose();
        }
    }

    private async Task ItemTappedAsync(ProductModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (item is not null)
                if (item.IsVariant)
                {
                    CurrentPage.FindByName<BottomSheet>("").State = BottomSheetState.HalfExpanded;
                }
                else
                {
                    if (!item.IsSelected)
                    {
                        Items.ToList().FirstOrDefault(x => x.ReferenceId == item.ReferenceId).IsSelected = true;

                        SelectedProduct = item;

                        var basketItem = new InputPurchaseBasketModel
                        {
                            ItemReferenceId = item.ReferenceId,
                            ItemCode = item.Code,
                            ItemName = item.Name,
                            UnitsetReferenceId = item.UnitsetReferenceId,
                            UnitsetCode = item.UnitsetCode,
                            UnitsetName = item.UnitsetName,
                            SubUnitsetReferenceId = item.SubUnitsetReferenceId,
                            SubUnitsetCode = item.SubUnitsetCode,
                            SubUnitsetName = item.SubUnitsetName,
                            IsSelected = false,
                            MainItemCode = string.Empty,
                            MainItemName = string.Empty,
                            MainItemReferenceId = default,
                            StockQuantity = item.StockQuantity,
                            Quantity = item.LocTracking == 0 ? 1 : 0,
                            InputQuantity = item.LocTracking == 0 ? 1 : 0,
                            LocTracking = item.LocTracking,
                            TrackingType = item.TrackingType,
                            IsVariant = item.IsVariant
                        };

                        SelectedProducts.Add(basketItem);
                    }
                    else
                    {
                        SelectedProduct = null;
                        var selectedItem = SelectedProducts.FirstOrDefault(x => x.ItemReferenceId == item.ReferenceId);
                        if (selectedItem != null)
                        {
                            SelectedProducts.Remove(selectedItem);
                            Items.ToList().FirstOrDefault(x => x.ReferenceId == item.ReferenceId).IsSelected = false;
                        }
                    }
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

    private async Task ConfirmAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            var previouseViewModel = _serviceProvider.GetRequiredService<InputProductPurchaseOrderProcessBasketListViewModel>();
            if (previouseViewModel is not null)
            {
                foreach (var item in SelectedProducts)
                    if (!previouseViewModel.Items.Any(x => x.ItemCode == item.ItemCode))
                        previouseViewModel.Items.Add(item);

                await Shell.Current.GoToAsync($"..");
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
}