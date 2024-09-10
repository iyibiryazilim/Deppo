using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.VariantModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Converters;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.ViewModels;
using DevExpress.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;

public partial class InputProductPurchaseProcessProductListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IProductService _productService;
    private readonly IVariantService _variantService;
    private readonly IUserDialogs _userDialogs;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    private ProductModel? selectedProduct;

    [ObservableProperty]
    private ObservableCollection<InputProductBasketModel> selectedProducts = new();

    public InputProductPurchaseProcessProductListViewModel(IHttpClientService httpClientService,
        IProductService productService,
        IVariantService variantService,
        IUserDialogs userDialogs,
        IServiceProvider serviceProvider)
    {
        _httpClientService = httpClientService;
        _productService = productService;
        _variantService = variantService;
        _userDialogs = userDialogs;
        _serviceProvider = serviceProvider;

        Title = "Ürün Listesi";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<object>(ItemTappedAsync);
        ShowSelectedProductsCommand = new Command<BottomSheet>(async (parameter) => await ShowSelectedProductsAsync(parameter));
        LoadVariantItemsCommand = new Command<ProductModel>(async (parameter) => await LoadVariantItemsAsync(parameter));
        LoadMoreVariantItemsCommand = new Command(async () => await LoadMoreVariantItemsAsync());
        VariantTappedCommand = new Command<VariantModel>(async (parameter) => await VariantTappedAsync(parameter));
        ConfirmVariantCommand = new Command(async () => await ConfirmVariantAsync());
        ConfirmCommand = new Command(async () => await ConfirmAsync());
        BackCommand = new Command(async () => await BackAsync());
    }

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command ShowSelectedProductsCommand { get; }
    public Command<ProductModel> LoadVariantItemsCommand { get; }
    public Command LoadMoreVariantItemsCommand { get; }
    public Command<VariantModel> VariantTappedCommand { get; }
    public Command ConfirmVariantCommand { get; }
    public Command ConfirmCommand { get; }
    public Command BackCommand { get; }

    public ObservableCollection<ProductModel> Items { get; } = new();
    public ObservableCollection<VariantModel> ItemVariants { get; } = new();

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

    private async void ItemTappedAsync(object param)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            SelectProductModel selectProductModel = (SelectProductModel)param;
            ProductModel item = Items.FirstOrDefault(x => x.ReferenceId == selectProductModel.ItemReferenceId);
            BottomSheet variantBottomSheet = selectProductModel.BottomSheet;

            if (item is not null)
                if (item.IsVariant)
                {
                    variantBottomSheet.State = BottomSheetState.HalfExpanded;
                }
                else
                {
                    if (!item.IsSelected)
                    {
                        Items.ToList().FirstOrDefault(x => x.ReferenceId == item.ReferenceId).IsSelected = true;

                        SelectedProduct = item;

                        var basketItem = new InputProductBasketModel
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

    private async Task ShowSelectedProductsAsync(BottomSheet bottomSheet)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            bottomSheet.State = BottomSheetState.HalfExpanded;
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

    private async Task LoadVariantItemsAsync(ProductModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            Items.Clear();
            SelectedProducts.Clear();

            _userDialogs.Loading("Loading Variant Items...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            await Task.Delay(1000);
            var result = await _variantService
                                .GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, item.ReferenceId, WarehouseModel.Number, string.Empty, 0, 20);

            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var variant in result.Data)
                    ItemVariants.Add(variant);
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

    private async Task LoadMoreVariantItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.Loading("Loading More Variant Items...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _variantService
                                .GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SelectedProduct.ReferenceId, WarehouseModel.Number, string.Empty, 0, 20);

            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var variant in result.Data)
                    ItemVariants.Add(variant);
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

    private async Task VariantTappedAsync(VariantModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            ItemVariants.ToList().ForEach(x => x.IsSelected = false);
            var selectedItem = ItemVariants.FirstOrDefault(x => x.ReferenceId == item.ReferenceId);
            if (selectedItem != null)
                selectedItem.IsSelected = true;
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

    private async Task ConfirmVariantAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            var item = ItemVariants.FirstOrDefault(x => x.IsSelected);
            var basketItem = new InputProductBasketModel
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
                MainItemCode = item.ProductCode,
                MainItemName = item.ProductName,
                MainItemReferenceId = item.ProductReferenceId,
                StockQuantity = item.StockQuantity,
                Quantity = item.LocTracking == 0 ? 1 : 0,
                TrackingType = item.TrackingType,
                LocTracking = item.LocTracking,
            };

            SelectedProducts.Add(basketItem);
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

            var previouseViewModel = _serviceProvider.GetRequiredService<InputProductPurchaseProcessBasketListViewModel>();
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

    private async Task BackAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

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
}