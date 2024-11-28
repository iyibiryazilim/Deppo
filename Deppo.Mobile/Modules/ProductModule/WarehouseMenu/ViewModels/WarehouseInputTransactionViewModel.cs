using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Helpers.QueryHelper;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.ProductModule.WarehouseMenu.ViewModels;

[QueryProperty(name: nameof(Warehouse), queryId: nameof(Warehouse))]
public partial class WarehouseInputTransactionViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ICustomQueryService _customQueryService;
    private readonly IWarehouseInputTransactionService _warehouseInputTransactionService;
    private readonly IUserDialogs _userDialogs;

    #region Properties

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    public Product selectedItem;

    [ObservableProperty]
    private Warehouse warehouse = null!;

    public Page CurrentPage { get; set; }

    #endregion Properties

    #region Collections

    public ObservableCollection<Product> Items { get; } = new();

    public ObservableCollection<WarehouseTransactionModel> Transactions { get; } = new();

    #endregion Collections

    public WarehouseInputTransactionViewModel(IHttpClientService httpClientService, ICustomQueryService customQueryService, IWarehouseInputTransactionService warehouseInputTransactionService, IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _customQueryService = customQueryService;
        _warehouseInputTransactionService = warehouseInputTransactionService;
        _userDialogs = userDialogs;

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<Product>(async (item) => await ItemTappedAsync(item));
        TransactionCloseCommand = new Command(async () => await TransactionCloseAsync());
        LoadMoreTransactionsCommand = new Command(async () => await LoadMoreTransactionsAsync());
        BackCommand = new Command(async () => await BackAsync());
    }

    #region Commands

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command<SearchBar> PerformSearchCommand { get; }
    public Command BackCommand { get; }
    public Command ItemTappedCommand { get; }

    public Command TransactionCloseCommand { get; }

    public Command LoadMoreTransactionsCommand { get; }

    #endregion Commands

    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Loading...");
            Items.Clear(); // İlk önce mevcut öğeleri temizle
            await Task.Delay(1000); // Yükleme gecikmesi (örnek için)

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseInputTransactionService.GetWarehouseInputProducts(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, Warehouse.Number, "", 0, 20);

            if (result.IsSuccess && result.Data is not null)
            {
                // Daha önce eklenmiş ürünlerin kontrolü için bir liste
                var existingProductIds = Items.Select(p => p.ReferenceId).ToHashSet();

                foreach (var item in result.Data)
                {
                    var warehouseProduct = Mapping.Mapper.Map<WarehouseTransactionModel>(item);

                    // Eğer ürün daha önce eklenmemişse
                    if (!existingProductIds.Contains(warehouseProduct.ProductReferenceId))
                    {
                        Items.Add(new Product
                        {
                            ReferenceId = warehouseProduct.ProductReferenceId,
                            Code = warehouseProduct.ProductCode,
                            Name = warehouseProduct.ProductName,
                            StockQuantity = warehouseProduct.Quantity,
                            UnitsetReferenceId = warehouseProduct.UnitsetReferenceId,
                            UnitsetName = warehouseProduct.UnitsetName,
                            UnitsetCode = warehouseProduct.UnitsetCode,
                            SubUnitsetReferenceId = warehouseProduct.SubUnitsetReferenceId,
                            SubUnitsetName = warehouseProduct.SubUnitsetName,
                            SubUnitsetCode = warehouseProduct.SubUnitsetCode,
                            LocTracking = warehouseProduct.LocTracking,
                            TrackingType = warehouseProduct.TrackingType,
                            IsVariant = warehouseProduct.IsVariant,
                            Image = warehouseProduct.Image,
                        });
                    }
                }
            }

            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message);
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
            _userDialogs.ShowLoading("Loading...");
            await Task.Delay(50);
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseInputTransactionService.GetWarehouseInputProducts(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, Warehouse.Number, "", Items.Count, 20);

            if (result.IsSuccess && result.Data is not null)
            {
                // Daha önce eklenmiş ürünlerin kontrolü için bir liste
                var existingProductIds = Items.Select(p => p.ReferenceId).ToHashSet();

                foreach (var item in result.Data)
                {
                    var warehouseProduct = Mapping.Mapper.Map<WarehouseTransactionModel>(item);

                    // Eğer ürün daha önce eklenmemişse
                    if (!existingProductIds.Contains(warehouseProduct.ProductReferenceId))
                    {
                        Items.Add(new Product
                        {
                            ReferenceId = warehouseProduct.ProductReferenceId,
                            Code = warehouseProduct.ProductCode,
                            Name = warehouseProduct.ProductName,
                            StockQuantity = warehouseProduct.Quantity,
                            UnitsetReferenceId = warehouseProduct.UnitsetReferenceId,
                            UnitsetName = warehouseProduct.UnitsetName,
                            UnitsetCode = warehouseProduct.UnitsetCode,
                            SubUnitsetReferenceId = warehouseProduct.SubUnitsetReferenceId,
                            SubUnitsetName = warehouseProduct.SubUnitsetName,
                            SubUnitsetCode = warehouseProduct.SubUnitsetCode,
                            LocTracking = warehouseProduct.LocTracking,
                            TrackingType = warehouseProduct.TrackingType,
                            IsVariant = warehouseProduct.IsVariant
                        });
                    }
                }
            }

            _userDialogs.HideHud();
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ItemTappedAsync(Product product)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            SelectedItem = product;

            await LoadTransactionsAsync(product);
            CurrentPage.FindByName<BottomSheet>("transactionBottomSheet").State = BottomSheetState.HalfExpanded;
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

    private async Task TransactionCloseAsync()
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            CurrentPage.FindByName<BottomSheet>("transactionBottomSheet").State = BottomSheetState.Hidden;
        });
    }

    private async Task LoadTransactionsAsync(Product product)
    {
        try
        {
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);

            Transactions.Clear();

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseInputTransactionService.GetWarehouseInputTransactions(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, product.ReferenceId, "", 0, 20);
            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                    Transactions.Add(Mapping.Mapper.Map<WarehouseTransactionModel>(item));
            }

            _userDialogs.HideHud();
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
    }

    private async Task LoadMoreTransactionsAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseInputTransactionService.GetWarehouseInputTransactions(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SelectedItem.ReferenceId, "", Transactions.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                    Transactions.Add(Mapping.Mapper.Map<WarehouseTransactionModel>(item));
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

            await Task.Delay(300);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(message: ex.Message, title: "Hata");
        }
        finally
        {
            IsBusy = false;
        }
    }
}