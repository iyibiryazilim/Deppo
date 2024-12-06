using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.VariantModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class OutputProductProcessProductListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IWarehouseTotalService _warehouseTotalService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IVariantWarehouseTotalService _variantWarehouseTotalService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

    public ObservableCollection<WarehouseTotalModel> Items { get; } = new();
    public ObservableCollection<WarehouseTotalModel> SelectedItems { get; } = new();
    public ObservableCollection<VariantWarehouseTotalModel> ItemVariants { get; } = new();

    [ObservableProperty]
    public ObservableCollection<OutputProductBasketModel> selectedProducts = new();

    [ObservableProperty]
    private WarehouseTotalModel selectedProduct = null!;

    public ContentPage CurrentPage { get; set; } = null!;

    private bool IsSearchMode
    {
        get
        {
            if (CurrentPage is OutputProductProcessProductListView)
            {
                SearchBar searchBar = (SearchBar)CurrentPage.FindByName("searchBar");
                if (searchBar is not null)
                {
                    if (string.IsNullOrEmpty(searchBar.Text))
                        return false;
                    else
                        return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
    }

    public OutputProductProcessProductListViewModel(IHttpClientService httpClientService, IWarehouseTotalService warehouseTotalService, IServiceProvider serviceProvider, IVariantWarehouseTotalService variantWarehouseTotalService, IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _warehouseTotalService = warehouseTotalService;
        _serviceProvider = serviceProvider;
        _variantWarehouseTotalService = variantWarehouseTotalService;
        _userDialogs = userDialogs;

        Title = "Ürün Listesi";
        SelectedItems.Clear();
        SelectedProducts.Clear();

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<WarehouseTotalModel>(async (parameter) => await ItemTappedAsync(parameter));
        LoadVariantItemsCommand = new Command(async () => await LoadVariantItemsAsync());
        LoadMoreVariantItemsCommand = new Command(async () => await LoadMoreVariantItemsAsync());
        VariantTappedCommand = new Command<VariantWarehouseTotalModel>(async (parameter) => await VariantTappedAsync(parameter));
        ConfirmVariantCommand = new Command(async () => await ConfirmVariantAsync());
        ConfirmCommand = new Command(async () => await ConfirmAsync());
        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
        BackCommand = new Command(async () => await BackAsync());
    }

    #region Commands

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command ConfirmCommand { get; }
    public Command PerformSearchCommand { get; }
    public Command PerformEmptySearchCommand { get; }
    public Command BackCommand { get; }

    public Command LoadVariantItemsCommand { get; }
    public Command LoadMoreVariantItemsCommand { get; }
    public Command VariantTappedCommand { get; }
    public Command ConfirmVariantCommand { get; }

    #endregion Commands

    [ObservableProperty]
    public SearchBar searchText;

    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            _userDialogs.Loading("Loading Items...");
            Items.Clear();
            await Task.Delay(1000);
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseTotalService.GetObjects(httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, warehouseNumber: WarehouseModel.Number, SearchText.Text, 0, 20);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var product in result.Data)
                {
                    var item = Mapping.Mapper.Map<WarehouseTotal>(product);

                    var matchingItem = SelectedItems.FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId);
                    Items.Add(new WarehouseTotalModel
                    {
                        ProductReferenceId = item.ProductReferenceId,
                        ProductCode = item.ProductCode,
                        ProductName = item.ProductName,
                        UnitsetReferenceId = item.UnitsetReferenceId,
                        UnitsetCode = item.UnitsetCode,
                        UnitsetName = item.UnitsetName,
                        SubUnitsetReferenceId = item.SubUnitsetReferenceId,
                        SubUnitsetCode = item.SubUnitsetCode,
                        SubUnitsetName = item.SubUnitsetName,
                        StockQuantity = item.StockQuantity,
                        WarehouseReferenceId = item.WarehouseReferenceId,
                        WarehouseName = item.WarehouseName,
                        WarehouseNumber = item.WarehouseNumber,
                        LocTracking = item.LocTracking,
                        IsVariant = item.IsVariant,
                        TrackingType = item.TrackingType,
                        IsSelected = matchingItem != null ? matchingItem.IsSelected : false,
                        LocTrackingIcon = product.LocTrackingIcon,
                        VariantIcon = product.VariantIcon,
                        Image = product.Image,
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
        if (Items.Count < 18)
            return;
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            _userDialogs.Loading("Loading More Items...");

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseTotalService.GetObjects(httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, warehouseNumber: WarehouseModel.Number, SearchText.Text, skip: Items.Count, take: 20);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var product in result.Data)
                {
                    var item = Mapping.Mapper.Map<WarehouseTotal>(product);
                    var matchingItem = SelectedItems.FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId);
                    Items.Add(new WarehouseTotalModel
                    {
                        ProductReferenceId = item.ProductReferenceId,
                        ProductCode = item.ProductCode,
                        ProductName = item.ProductName,
                        UnitsetReferenceId = item.UnitsetReferenceId,
                        UnitsetCode = item.UnitsetCode,
                        UnitsetName = item.UnitsetName,
                        SubUnitsetReferenceId = item.SubUnitsetReferenceId,
                        SubUnitsetCode = item.SubUnitsetCode,
                        SubUnitsetName = item.SubUnitsetName,
                        StockQuantity = item.StockQuantity,
                        WarehouseReferenceId = item.WarehouseReferenceId,
                        WarehouseName = item.WarehouseName,
                        WarehouseNumber = item.WarehouseNumber,
                        LocTracking = item.LocTracking,
                        IsVariant = item.IsVariant,
                        TrackingType = item.TrackingType,
                        IsSelected = matchingItem != null ? matchingItem.IsSelected : false,
                        LocTrackingIcon = product.LocTrackingIcon,
                        VariantIcon = product.VariantIcon,
                        TrackingTypeIcon = product.TrackingTypeIcon,
                        Image = item.Image
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

    private async Task ItemTappedAsync(WarehouseTotalModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
			SelectedProduct = item;

			if (item is not null)
            {
                if (!item.IsSelected)
                {
                    if (item.IsVariant)
                    {
                        SelectedProduct = item;
                        await LoadVariantItemsAsync();
                        CurrentPage.FindByName<BottomSheet>("variantBottomSheet").State = BottomSheetState.HalfExpanded;
                    }
                    else
                    {
                        Items.ToList().FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId).IsSelected = true;
                        SelectedProduct = item;
                        SelectedItems.Add(SelectedProduct);

                        var basketItem = new OutputProductBasketModel
                        {
                            ItemReferenceId = item.ProductReferenceId,
                            ItemCode = item.ProductCode,
                            ItemName = item.ProductName,
                            UnitsetReferenceId = item.UnitsetReferenceId,
                            UnitsetCode = item.UnitsetCode,
                            UnitsetName = item.UnitsetName,
                            SubUnitsetReferenceId = item.SubUnitsetReferenceId,
                            SubUnitsetCode = item.SubUnitsetCode,
                            SubUnitsetName = item.SubUnitsetName,
                            MainItemReferenceId = default,  //
                            MainItemCode = string.Empty,    //
                            MainItemName = string.Empty,    //
                            StockQuantity = item.StockQuantity,
                            IsSelected = false,   //
                            IsVariant = item.IsVariant,
                            LocTracking = item.LocTracking,
                            TrackingType = item.TrackingType,
                            Quantity = item.LocTracking == 0 ? 1 : 0,
                            LocTrackingIcon = item.LocTrackingIcon,
                            VariantIcon = item.VariantIcon,
                            TrackingTypeIcon = item.TrackingTypeIcon,
                            Image = item.ImageData,
                        };

                        SelectedProducts.Add(basketItem);
                        SelectedItems.Add(item);
                    }
                }
                else
                {
                    SelectedProduct.IsSelected = false;
                    SelectedProduct = null;
                    var selectedItem = SelectedProducts.FirstOrDefault(x => x.ItemReferenceId == item.ProductReferenceId);
                    if (selectedItem is not null)
                    {
                        SelectedProducts.Remove(selectedItem);
                        Items.ToList().FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId).IsSelected = false;
                        SelectedItems.Remove(item);
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

    private async Task LoadVariantItemsAsync()
    {
        try
        {
            _userDialogs.Loading("Loading Variant Items");
            ItemVariants.Clear();
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _variantWarehouseTotalService.GetObjects(httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, productReferenceId: SelectedProduct.ProductReferenceId, warehouseNumber: WarehouseModel.Number, skip: 0, take: 20);

            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;
                ItemVariants.Clear();
                foreach (var variant in result.Data)
                {
                    var obj = Mapping.Mapper.Map<VariantWarehouseTotalModel>(variant);
                    ItemVariants.Add(obj);
                }
            }

            _userDialogs.Loading().Hide();
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
        finally
        {
        }
    }

    private async Task LoadMoreVariantItemsAsync()
    {
        if (ItemVariants.Count < 18)
            return;
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _variantWarehouseTotalService.GetObjects(httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, productReferenceId: SelectedProduct.ProductReferenceId, warehouseNumber: WarehouseModel.Number, skip: ItemVariants.Count(), take: 20);

            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var variant in result.Data)
                {
                    var obj = Mapping.Mapper.Map<VariantWarehouseTotalModel>(variant);
                    ItemVariants.Add(obj);
                }
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task VariantTappedAsync(VariantWarehouseTotalModel item)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            ItemVariants.ToList().ForEach(x => x.IsSelected = false);
            var selectedItem = ItemVariants.FirstOrDefault(x => x.VariantReferenceId == item.VariantReferenceId);
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
            var basketItem = new OutputProductBasketModel
            {
                ItemReferenceId = item.VariantReferenceId,
                ItemCode = item.VariantCode,
                ItemName = item.VariantName,
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
                IsVariant = true,
                Image = item.ImageData
            };

            SelectedProducts.Add(basketItem);

            if (SelectedProduct is not null)
            {
                SelectedProduct.IsSelected = true;
                SelectedItems.Add(SelectedProduct);
            }

            CurrentPage.FindByName<BottomSheet>("variantBottomSheet").State = BottomSheetState.Hidden;
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

            SearchText.Text = string.Empty;

            var previousViewModel = _serviceProvider.GetRequiredService<OutputProductProcessBasketListViewModel>();
            if (previousViewModel is not null)
            {
                if (SelectedProducts.Any())
                {
                    foreach (var item in SelectedProducts)
                    {
                        if (!previousViewModel.Items.Any(x => x.ItemCode == item.ItemCode))
                            previousViewModel.Items.Add(item);
                    }
                }
            }
            SearchText.Text = string.Empty;
            await Shell.Current.GoToAsync($"..");
            SelectedProducts.Clear();
            SelectedItems.Clear();
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

    private async Task PerformSearchAsync()
    {
        if (IsBusy)
            return;
        try
        {
            if (string.IsNullOrWhiteSpace(SearchText.Text))
            {
                await LoadItemsAsync();
                SearchText.Unfocus();
                return;
            }
            IsBusy = true;

            Items.Clear();
            _userDialogs.Loading("Searching Items...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseTotalService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, SearchText.Text, 0, 20);
            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var product in result.Data)
                {
                    var item = Mapping.Mapper.Map<WarehouseTotal>(product);
                    var matchedItem = SelectedItems.FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId);
                    Items.Add(new WarehouseTotalModel
                    {
                        ProductReferenceId = item.ProductReferenceId,
                        ProductCode = item.ProductCode,
                        ProductName = item.ProductName,
                        UnitsetReferenceId = item.UnitsetReferenceId,
                        UnitsetCode = item.UnitsetCode,
                        UnitsetName = item.UnitsetName,
                        SubUnitsetReferenceId = item.SubUnitsetReferenceId,
                        SubUnitsetCode = item.SubUnitsetCode,
                        SubUnitsetName = item.SubUnitsetName,
                        StockQuantity = item.StockQuantity,
                        WarehouseReferenceId = item.WarehouseReferenceId,
                        WarehouseName = item.WarehouseName,
                        WarehouseNumber = item.WarehouseNumber,
                        IsVariant = item.IsVariant,
                        LocTracking = item.LocTracking,
                        TrackingType = item.TrackingType,
                        IsSelected = matchedItem != null ? matchedItem.IsSelected : false,
                        LocTrackingIcon = product.LocTrackingIcon,
                        VariantIcon = product.VariantIcon,
                        TrackingTypeIcon = product.TrackingTypeIcon,
                        Image = product.Image,
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

    private async Task PerformEmptySearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText.Text))
        {
            await PerformSearchAsync();
        }
    }

    private async Task BackAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            if (SelectedProducts.Count > 0)
            {
                var result = await _userDialogs.ConfirmAsync("Seçtiğiniz ürünler silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
                if (!result)
                {
                    return;
                }
                SelectedProducts.Clear();
            }

            SearchText.Text = string.Empty;
            await Shell.Current.GoToAsync($"..");
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }
}