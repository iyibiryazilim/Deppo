using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.VirmanProductProcess.Views;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.VirmanProductProcess.ViewModels;

[QueryProperty(name: nameof(OutWarehouse), queryId: nameof(OutWarehouse))]
public partial class VirmanProductOutListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ISeriLotTransactionService _serilotTransactionService;
    private readonly IUserDialogs _userDialogs;
    private readonly IWarehouseTotalService _warehouseTotalService;

    [ObservableProperty]
    private WarehouseModel outWarehouse = null!;

    public ObservableCollection<WarehouseTotalModel> Items { get; } = new();

    [ObservableProperty]
    private WarehouseTotalModel? selectedProduct;

    public Page CurrentPage { get; set; }

    public VirmanProductOutListViewModel(IHttpClientService httpClientService, ISeriLotTransactionService serilotTransactionService, IUserDialogs userDialogs, IWarehouseTotalService warehouseTotalService)
    {
        _httpClientService = httpClientService;
        _serilotTransactionService = serilotTransactionService;
        _userDialogs = userDialogs;
        _warehouseTotalService = warehouseTotalService;

        Title = "Çıkış Ürünleri Listesi";

        BackCommand = new Command(async () => await BackAsync());
        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<WarehouseTotalModel>(async (parameter) => await ItemTappedAsync(parameter));
        NextViewCommand = new Command(async () => await NextViewAsync());
        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
    }

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command ConfirmCommand { get; }
    public Command BackCommand { get; }
    public Command NextViewCommand { get; }
    public Command PerformSearchCommand { get; }
    public Command PerformEmptySearchCommand { get; }

    [ObservableProperty]
    public SearchBar searchText;
    private async Task NextViewAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(VirmanProductInWarehouseListView)}", new Dictionary<string, object>
            {
                ["OutWarehouse"] = OutWarehouse,
                [nameof(WarehouseTotalModel)] = SelectedProduct
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

    private async Task ItemTappedAsync(WarehouseTotalModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (item is not null)
            {
                // Eğer item zaten seçiliyse, seçimi kaldır
                if (item == SelectedProduct)
                {
                    SelectedProduct.IsSelected = false;
                    SelectedProduct = null;
                }
                else
                {
                    // Daha önce seçili olan varsa, onun seçimini kaldır
                    if (SelectedProduct != null)
                    {
                        SelectedProduct.IsSelected = false;
                    }

                    // Yeni item'i seç ve IsSelected durumunu true yap
                    SelectedProduct = item;
                    SelectedProduct.IsSelected = true;

                    // Items listesindeki ilgili öğenin IsSelected durumunu güncelle
                    var selectedItem = Items.FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId);
                    if (selectedItem is not null)
                    {
                        selectedItem.IsSelected = true;
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

    private async Task LoadMoreItemsAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseTotalService.GetObjects(httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, warehouseNumber: OutWarehouse.Number, skip: Items.Count, take: 20,search: SearchText.Text);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var product in result.Data)
                {
                    var item = Mapping.Mapper.Map<WarehouseTotal>(product);

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
            var result = await _warehouseTotalService.GetObjects(httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, warehouseNumber: OutWarehouse.Number, SearchText.Text);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var product in result.Data)
                {
                    var item = Mapping.Mapper.Map<WarehouseTotal>(product);
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
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseTotalService.GetObjects(httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, warehouseNumber: OutWarehouse.Number,SearchText.Text);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var product in result.Data)
                {
                    var item = Mapping.Mapper.Map<WarehouseTotal>(product);
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
                        IsSelected = false,
                        LocTrackingIcon = product.LocTrackingIcon,
                        VariantIcon = product.VariantIcon,
                        TrackingTypeIcon = product.TrackingTypeIcon,
                    });
                }
            }
            if (!result.IsSuccess)
            {
                _userDialogs.Alert(result.Message, "Hata");
                return;
            }
        }
        catch (System.Exception ex)
        {
            _userDialogs.Alert(ex.Message, "Hata");
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
}