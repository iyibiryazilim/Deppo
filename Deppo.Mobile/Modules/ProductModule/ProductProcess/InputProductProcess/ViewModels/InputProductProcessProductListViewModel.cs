using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.VariantModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Converters;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;
using DevExpress.Maui.Controls;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class InputProductProcessProductListViewModel : BaseViewModel
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

    private bool IsSearchMode
    {
        get
        {
            if (CurrentPage is InputProductProcessProductListView)
            {
                SearchBar searchBar = (SearchBar)CurrentPage.FindByName("searchBar");
                if (searchBar is not null)
                    if (string.IsNullOrEmpty(searchBar.Text))
                        return false;
                    else
                        return true;
                else
                    return false;
            }
            else
                return false;
        }
    }

    public InputProductProcessProductListViewModel(
        IHttpClientService httpClientService,
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
        ItemTappedCommand = new Command<ProductModel>(async (parameter) => await ItemTappedAsync(parameter));
        ShowSelectedProductsCommand = new Command<BottomSheet>(async (parameter) => await ShowSelectedProductsAsync(parameter));
        LoadVariantItemsCommand = new Command<ProductModel>(async (parameter) => await LoadVariantItemsAsync(parameter));
        LoadMoreVariantItemsCommand = new Command(async () => await LoadMoreVariantItemsAsync());
        VariantTappedCommand = new Command<VariantModel>(async (parameter) => await VariantTappedAsync(parameter));
        ConfirmVariantCommand = new Command(async () => await ConfirmVariantAsync());
        ConfirmCommand = new Command(async () => await ConfirmAsync());
        BackCommand = new Command(async () => await BackAsync());
        PerformSearchCommand = new Command<SearchBar>(async (parameter) => await PerformSearchAsync(parameter));
    }

    public Page CurrentPage { get; set; } = null!;

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command<ProductModel> ItemTappedCommand { get; }
    public Command ShowSelectedProductsCommand { get; }
    public Command<ProductModel> LoadVariantItemsCommand { get; }
    public Command LoadMoreVariantItemsCommand { get; }
    public Command<VariantModel> VariantTappedCommand { get; }
    public Command ConfirmVariantCommand { get; }
    public Command ConfirmCommand { get; }
    public Command BackCommand { get; }
    public Command<SearchBar> PerformSearchCommand { get; }

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

            if (!IsSearchMode)
                SelectedProducts.Clear();

            await Task.Delay(1000);
            _userDialogs.Loading("Loading Items...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var result = await _productService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, string.Empty, 0, 20);
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
                        GroupCode = item.GroupCode,
                        BrandReferenceId = item.BrandReferenceId,
                        BrandCode = item.BrandCode,
                        BrandName = item.BrandName,
                        VatRate = item.VatRate,
                        Image = item.Image,
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
        if (IsSearchMode)
        {
            await PerformSearchMoreAsync(((SearchBar)CurrentPage.FindByName("searchBar")));
            return;
        }

        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.Loading("Loading More Items...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _productService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, string.Empty, Items.Count, 20);
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

    private async Task ItemTappedAsync(ProductModel productModel)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            ProductModel item = Items.FirstOrDefault(x => x.ReferenceId == productModel.ReferenceId);
            BottomSheet variantBottomSheet = (BottomSheet)CurrentPage.FindByName("variantBottomSheet");

            if (item is not null)
                if (item.IsVariant)
                {
                    if (variantBottomSheet is not null)
                        variantBottomSheet.State = BottomSheetState.HalfExpanded;
                }
                else
                {
                    if (!item.IsSelected)
                    {
                        var tappedItem = Items.ToList().FirstOrDefault(x => x.ReferenceId == item.ReferenceId);
                        if (tappedItem != null)
                            tappedItem.IsSelected = true;

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
                            IsVariant = item.IsVariant,
                            VariantIcon = item.VariantIcon,
                            LocTrackingIcon = item.LocTrackingIcon,
                            TrackingTypeIcon = item.TrackingTypeIcon
                            

                           
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

            var previouseViewModel = _serviceProvider.GetRequiredService<InputProductProcessBasketListViewModel>();
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

    private async Task PerformSearchAsync(SearchBar searchBar)
    {
        if (IsBusy)
            return;

        try
        {
            if (string.IsNullOrWhiteSpace(searchBar.Text))
            {
                await LoadItemsAsync();
                searchBar.Unfocus();
                return;
            }
            else
            {
                if (searchBar.Text.Length > 5)
                {
                    IsBusy = true;
                    Items.Clear();
                    _userDialogs.Loading("Searching Items...");
                    var httpClient = _httpClientService.GetOrCreateHttpClient();
                    await Task.Delay(1000);
                    var result = await _productService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, searchBar.Text, 0, 20);
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
                                GroupCode = item.GroupCode,
                                BrandReferenceId = item.BrandReferenceId,
                                BrandCode = item.BrandCode,
                                BrandName = item.BrandName,
                                VatRate = item.VatRate,
                                Image = item.Image,
                                IsVariant = item.IsVariant,
                                IsSelected = false
                            });
                        }
                    }

                    _userDialogs.Loading().Hide();
                    searchBar.Unfocus();
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

    private async Task PerformSearchMoreAsync(SearchBar searchBar)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.Loading("Searching Items...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            await Task.Delay(1000);
            var result = await _productService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, searchBar.Text, Items.Count, 20);
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
                        GroupCode = item.GroupCode,
                        BrandReferenceId = item.BrandReferenceId,
                        BrandCode = item.BrandCode,
                        BrandName = item.BrandName,
                        VatRate = item.VatRate,
                        Image = item.Image,
                        IsVariant = item.IsVariant,
                        IsSelected = false
                    });
                }
            }

            _userDialogs.Loading().Hide();
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