using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
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

    [ObservableProperty]
    private PurchaseFicheModel purchaseFicheModel = null!;

    [ObservableProperty]
    private PurchaseSupplier purchaseSupplier = null!;

    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

	public ObservableCollection<PurchaseTransactionModel> Items { get; } = new();

	public ObservableCollection<PurchaseTransactionModel> SelectedPurchaseTransactions { get; } = new();

	public ObservableCollection<ReturnPurchaseBasketModel> SelectedProducts = new();

	public ReturnPurchaseDispatchProductListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IPurchaseDispatchTransactionService purchaseDispatchTransactionService)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _purchaseDispatchTransactionService = purchaseDispatchTransactionService;


        Title = "Ürünler";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<PurchaseTransactionModel>(async (x) => await ItemTappedAsync(x));
        NextViewCommand = new Command(async () => await NextViewAsync());
    }

    public Page CurrentPage { get; set; } = null!;

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command NextViewCommand { get; }

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
            var result = await _purchaseDispatchTransactionService.GetTransactionsByFicheReferenceId(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, PurchaseFicheModel.ReferenceId,string.Empty, 0, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var transaction in result.Data)
                    {
                        var item = Mapping.Mapper.Map<PurchaseTransactionModel>(transaction);
                        Items.Add(item);
                    }
                }
            }

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

        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _purchaseDispatchTransactionService.GetTransactionsByFicheReferenceId(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, PurchaseFicheModel.ReferenceId,string.Empty, Items.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var transaction in result.Data)
                    {

                        var item = Mapping.Mapper.Map<PurchaseTransactionModel>(transaction);
                        Items.Add(item);   
                    }

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

    private async Task ItemTappedAsync(PurchaseTransactionModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            var selectedItem = Items.FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId);
            if (selectedItem is not null)
            {
                if (selectedItem.IsSelected)
                {
                    Items.FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId).IsSelected = false;
                    SelectedPurchaseTransactions.Remove(SelectedPurchaseTransactions.FirstOrDefault(x => x.ProductReferenceId == selectedItem.ProductReferenceId));
                }
                else
                {
                    Items.FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId).IsSelected = true;

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
					};

					SelectedProducts.Add(basketItem);


					SelectedPurchaseTransactions.Add(selectedItem);
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
            Console.WriteLine(SelectedProducts);

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
}
