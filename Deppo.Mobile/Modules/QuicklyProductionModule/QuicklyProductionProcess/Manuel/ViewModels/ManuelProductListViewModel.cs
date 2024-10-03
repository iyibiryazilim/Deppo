using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.QuicklyModels;
using Deppo.Mobile.Core.Models.QuicklyModels.BasketModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.Views;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.ViewModels;

public partial class ManuelProductListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ISeriLotTransactionService _serilotTransactionService;
    private readonly IUserDialogs _userDialogs;
    private readonly IQuicklyBomService _quicklyBomService;

    public ObservableCollection<QuicklyBOMProductModel> Items { get; } = new();

    [ObservableProperty]
    private QuicklyBOMProductModel? selectedProduct;

    [ObservableProperty]
    private QuicklyBomProductBasketModel? basketModel = new();

    public Page CurrentPage { get; set; }

    public ManuelProductListViewModel(IHttpClientService httpClientService, ISeriLotTransactionService serilotTransactionService, IUserDialogs userDialogs, IQuicklyBomService quicklyBomService)
    {
        _httpClientService = httpClientService;
        _serilotTransactionService = serilotTransactionService;
        _userDialogs = userDialogs;
        _quicklyBomService = quicklyBomService;

        Title = "Ürün Listesi";

        BackCommand = new Command(async () => await BackAsync());
        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<QuicklyBOMProductModel>(async (parameter) => await ItemTappedAsync(parameter));
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

            if (SelectedProduct is null)
            {
                await _userDialogs.AlertAsync("Lütfen bir ürün seçiniz.", "Hata", "Tamam");
                return;
            }
            BasketModel.QuicklyBomProduct = SelectedProduct;
            BasketModel.WarehouseNumber = SelectedProduct.WarehouseNumber;
            BasketModel.WarehouseName = SelectedProduct.WarehouseName;
            await Shell.Current.GoToAsync($"{nameof(ManuelCalcInWarehouseListView)}", new Dictionary<string, object>
            {
                [nameof(QuicklyBomProductBasketModel)] = BasketModel
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

    private async Task ItemTappedAsync(QuicklyBOMProductModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (item is not null)
            {
                if (item == SelectedProduct)
                {
                    SelectedProduct.IsSelected = false;
                    SelectedProduct = null;
                }
                else
                {
                    if (SelectedProduct != null)
                    {
                        SelectedProduct.IsSelected = false;
                    }

                    SelectedProduct = item;
                    SelectedProduct.IsSelected = true;

                    var selectedItem = Items.FirstOrDefault(x => x.ReferenceId == item.ReferenceId);
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
            _userDialogs.ShowLoading("Loading...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _quicklyBomService.GetObjects(httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, skip: Items.Count, take: 20,search:SearchText.Text);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var product in result.Data)
                {
                    var item = Mapping.Mapper.Map<QuicklyBOMProductModel>(product);

                    Items.Add(item);
                    /* Items.Add(new QuicklyBOMProductModel
                     {
                         ReferenceId = item.ProductReferenceId,
                         Code = item.ProductCode,
                         Name = item.ProductName,
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
                     });*/
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
                if(BasketModel is not null)
                {
                    BasketModel.WarehouseNumber = default;
                    BasketModel.WarehouseName = string.Empty;
                    BasketModel.BOMQuantity = default;
                    if(BasketModel.QuicklyBomProduct is not null)
                    BasketModel.QuicklyBomProduct = null;
                }
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
            var result = await _quicklyBomService.GetObjects(httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber,search:SearchText.Text);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var product in result.Data)
                {
                    var item = Mapping.Mapper.Map<QuicklyBOMProductModel>(product);

                    Items.Add(item);
                    /* Items.Add(new QuicklyBOMProductModel
                     {
                         ReferenceId = (int)item.ProductReferenceId,
                         Code = item.ProductCode,
                         Name = item.ProductName,
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
                     });*/
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
            _userDialogs.Loading("Searching Items...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _quicklyBomService.GetObjects(httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, search: SearchText.Text);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var product in result.Data)
                {
                    var item = Mapping.Mapper.Map<QuicklyBOMProductModel>(product);

                    Items.Add(item);
                  
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
}