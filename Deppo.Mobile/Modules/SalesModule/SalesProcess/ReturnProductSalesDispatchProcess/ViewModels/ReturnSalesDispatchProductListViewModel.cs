using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesDispatchProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesProcess.ViewModels;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesDispatchProcess.ViewModels;

[QueryProperty(name: nameof(SalesFicheModel), queryId: nameof(SalesFicheModel))]
[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(SalesCustomer), queryId: nameof(SalesCustomer))]
public partial class ReturnSalesDispatchProductListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly ISalesDispatchTransactionService _salesDispatchTransactionService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private SalesFicheModel salesFicheModel = null!;

    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    private SalesCustomer salesCustomer = null!;

    public ReturnSalesDispatchProductListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, ISalesDispatchTransactionService salesDispatchTransactionService, IServiceProvider serviceProvider)
    {
        _httpClientService = httpClientService;
        Title = "Ürün Listesi";
        _userDialogs = userDialogs;
        _salesDispatchTransactionService = salesDispatchTransactionService;

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<SalesTransactionModel>(async (x) => await ItemTappedAsync(x));
        NextViewCommand = new Command(async () => await NextViewAsync());
        LoadPageCommand = new Command(async () => await LoadPageAsync());
        _serviceProvider = serviceProvider;
    }

    

    public ObservableCollection<SalesTransactionModel> Items { get; } = new();

    public ObservableCollection<SalesTransactionModel> SelectedSalesTransactions { get; } = new();
    public ObservableCollection<ReturnSalesBasketModel> SelectedProducts = new();

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }

    public Command ItemTappedCommand { get; }

    public Command NextViewCommand { get; }

    public Command LoadPageCommand { get; }

    


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
            var result = await _salesDispatchTransactionService.GetTransactionsByFicheReferenceId(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SalesFicheModel.ReferenceId, string.Empty, 0, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var transaction in result.Data)
                    {
                        var item = Mapping.Mapper.Map<SalesTransactionModel>(transaction);
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
            var result = await _salesDispatchTransactionService.GetTransactionsByFicheReferenceId(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SalesFicheModel.ReferenceId, string.Empty, Items.Count, 20);
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

    private async Task ItemTappedAsync(SalesTransactionModel item)
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
                    SelectedSalesTransactions.Remove(SelectedSalesTransactions.FirstOrDefault(x => x.ProductReferenceId == selectedItem.ProductReferenceId));
                }
                else
                {
                    Items.FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId).IsSelected = true;

                    var basketItem = new ReturnSalesBasketModel
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
                        Quantity = item.Quantity,
                        LocTrackingIcon = item.LocTrackingIcon,
                        VariantIcon = item.VariantIcon,
                        TrackingTypeIcon = item.TrackingTypeIcon,
                        DispatchReferenceId = item.ReferenceId,
                    };

                    SelectedProducts.Add(basketItem);


                    SelectedSalesTransactions.Add(selectedItem);
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
            var viewModel = _serviceProvider.GetRequiredService<ReturnSalesDispatchBasketViewModel>();
           // await viewModel.LoadPageAsync();
            await Shell.Current.GoToAsync($"{nameof(ReturnSalesDispatchBasketView)}", new Dictionary<string, object>
            {
                [nameof(Items)] = SelectedProducts,
                [nameof(WarehouseModel)] = WarehouseModel,
                [nameof(SalesCustomer)] = SalesCustomer,
                [nameof(SelectedSalesTransactions)] = SelectedSalesTransactions,
                [nameof(SalesFicheModel)] = SalesFicheModel
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
    private async Task LoadPageAsync()
    {
        
            try
            {


                if (Items?.Count > 0)
                    Items.Clear();
            }
            catch (Exception ex)
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

                await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
            }

        }
    
}