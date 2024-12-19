using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseDispatchProcess.Views;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseDispatchProcess.ViewModels;

[QueryProperty(name: nameof(PurchaseFicheModel), queryId: nameof(PurchaseFicheModel))]
[QueryProperty(name: nameof(PurchaseSupplier), queryId: nameof(PurchaseSupplier))]
[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class ReturnPurchaseDispatchProductListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly IPurchaseDispatchTransactionService _purchaseDispatchTransactionService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private PurchaseFicheModel purchaseFicheModel = null!;

    [ObservableProperty]
    private PurchaseSupplier purchaseSupplier = null!;

    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

    public ObservableCollection<PurchaseTransactionModel> Items { get; } = new();
    public ObservableCollection<PurchaseTransactionModel> SelectedItems { get; } = new();

    public ObservableCollection<PurchaseTransactionModel> SelectedPurchaseTransactions { get; } = new();

    public ObservableCollection<ReturnPurchaseBasketModel> SelectedProducts = new();

    public ReturnPurchaseDispatchProductListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IPurchaseDispatchTransactionService purchaseDispatchTransactionService, IServiceProvider serviceProvider)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _purchaseDispatchTransactionService = purchaseDispatchTransactionService;
        _serviceProvider = serviceProvider;

        Title = "Ürünler";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<PurchaseTransactionModel>(async (x) => await ItemTappedAsync(x));
        NextViewCommand = new Command(async () => await NextViewAsync());
        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
		BackCommand = new Command(async () => await BackAsync());
	}

    public Page CurrentPage { get; set; } = null!;

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command NextViewCommand { get; }
    public Command PerformSearchCommand { get; }
    public Command PerformEmptySearchCommand { get; }
    public Command BackCommand { get; }

    [ObservableProperty]
    public SearchBar searchText;

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
            var result = await _purchaseDispatchTransactionService.GetTransactionsByFicheReferenceId(
                httpClient: httpClient, 
                firmNumber: _httpClientService.FirmNumber, 
                periodNumber: _httpClientService.PeriodNumber, 
                ficheReferenceId: PurchaseFicheModel.ReferenceId, 
                search: SearchText.Text, 
                skip: 0, 
                take: 20,
                externalDb: _httpClientService.ExternalDatabase);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var transaction in result.Data)
                    {
                        var item = Mapping.Mapper.Map<PurchaseTransactionModel>(transaction);
                        var matchedItem = SelectedItems.FirstOrDefault(x => x.ReferenceId == item.ReferenceId);
                        if (matchedItem is not null)
                            item.IsSelected = matchedItem.IsSelected;
                        else
                            item.IsSelected = false;
                        Items.Add(item);
                    }
                }
            }

            if (_userDialogs.IsHudShowing)
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
        if (Items.Count < 18)
            return;

        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _purchaseDispatchTransactionService.GetTransactionsByFicheReferenceId(
                httpClient: httpClient, 
                firmNumber: _httpClientService.FirmNumber, 
                periodNumber: _httpClientService.PeriodNumber, 
                ficheReferenceId: PurchaseFicheModel.ReferenceId, 
                search: SearchText.Text, 
                skip: Items.Count, 
                take: 20,
                externalDb: _httpClientService.ExternalDatabase);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var transaction in result.Data)
                    {
                        var item = Mapping.Mapper.Map<PurchaseTransactionModel>(transaction);
                        var matchedItem = SelectedItems.FirstOrDefault(x => x.ReferenceId == item.ReferenceId);
                        if (matchedItem is not null)
                            item.IsSelected = matchedItem.IsSelected;
                        else
                            item.IsSelected = false;
                        Items.Add(item);
                    }
                }
            }

            if(_userDialogs.IsHudShowing)
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

    private async Task ItemTappedAsync(PurchaseTransactionModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            var selectedItem = Items.FirstOrDefault(x => x.ReferenceId == item.ReferenceId);
            if (selectedItem is not null)
            {
                if (selectedItem.IsSelected)
                {
                    Items.FirstOrDefault(x => x.ReferenceId == item.ReferenceId).IsSelected = false;
                    SelectedPurchaseTransactions.Remove(SelectedPurchaseTransactions.FirstOrDefault(x => x.ReferenceId == selectedItem.ReferenceId));
                    SelectedProducts.Remove(SelectedProducts.FirstOrDefault(x => x.ItemReferenceId == selectedItem.ProductReferenceId));
                    SelectedItems.Remove(selectedItem);
                }
                else
                {
                    Items.FirstOrDefault(x => x.ReferenceId == item.ReferenceId).IsSelected = true;

                    bool isExistingItem = SelectedProducts.Any(x => x.ItemReferenceId == item.ProductReferenceId);

                    if (isExistingItem)
                    {
                        var existingItem = SelectedProducts.FirstOrDefault(x => x.ItemReferenceId == item.ProductReferenceId);
                        existingItem.StockQuantity += item.Quantity;
                    }
                    else
                    {
                        var basketItem = new ReturnPurchaseBasketModel
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
                            StockQuantity = item.Quantity,
                            IsSelected = false,   //
                            IsVariant = item.IsVariant,
                            LocTracking = item.LocTracking,
                            TrackingType = item.TrackingType,
                            Quantity = item.LocTracking == 0 ? 1 : 0,
                            LocTrackingIcon = item.LocTrackingIcon,
                            VariantIcon = item.VariantIcon,
                            TrackingTypeIcon = item.TrackingTypeIcon,
                            DispatchReferenceId = item.ReferenceId,
                            Image = item.ImageData,
                        };
                        SelectedProducts.Add(basketItem);
                    }
                    SelectedPurchaseTransactions.Add(selectedItem);
                    SelectedItems.Add(selectedItem);
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

            SelectedItems.Clear();

            if (SelectedPurchaseTransactions.Count == 0)
            {
                await _userDialogs.AlertAsync("Devam etmek için lütfen en az bir ürün seçiniz.", "Uyarı", "Tamam");
                return;
            }

            await Shell.Current.GoToAsync($"{nameof(ReturnPurchaseDispatchBasketView)}", new Dictionary<string, object>
            {
                [nameof(WarehouseModel)] = WarehouseModel,
                [nameof(PurchaseFicheModel)] = PurchaseFicheModel,
                [nameof(PurchaseSupplier)] = PurchaseSupplier,
                [nameof(Items)] = SelectedProducts,
                [nameof(SelectedPurchaseTransactions)] = SelectedPurchaseTransactions,
            });
        }
        catch (System.Exception)
        {
            throw;
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
            var result = await _purchaseDispatchTransactionService.GetTransactionsByFicheReferenceId(
                httpClient: httpClient, 
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber, 
                ficheReferenceId: PurchaseFicheModel.ReferenceId, 
                search: SearchText.Text, 
                skip: 0, 
                take: 20,
                externalDb: _httpClientService.ExternalDatabase);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var transaction in result.Data)
                    {
                        var item = Mapping.Mapper.Map<PurchaseTransactionModel>(transaction);
                        var matchedItem = SelectedItems.FirstOrDefault(x => x.ReferenceId == item.ReferenceId);
                        if (matchedItem is not null)
                            item.IsSelected = matchedItem.IsSelected;
                        else
                            item.IsSelected = false;
                        Items.Add(item);
                    }
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
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

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

    private async Task BackAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            if(_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
        finally
        {
            IsBusy = false;
        }
    }
}