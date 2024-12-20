using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
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
        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
        _serviceProvider = serviceProvider;
    }

    public ObservableCollection<SalesTransactionModel> Items { get; } = new();

    // Arama için seçilenlerin tutulduğu liste
    public ObservableCollection<SalesTransactionModel> SelectedItems { get; } = new();

    public ObservableCollection<SalesTransactionModel> SelectedSalesTransactions { get; } = new();
    public ObservableCollection<ReturnSalesBasketModel> SelectedProducts = new();

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }

    public Command ItemTappedCommand { get; }

    public Command NextViewCommand { get; }

    public Command LoadPageCommand { get; }
    public Command PerformSearchCommand { get; }
    public Command PerformEmptySearchCommand { get; }
    public Command LoadMoreVariantItemsCommand { get; }
    public Command ConfirmVariantCommand { get; }

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
            var result = await _salesDispatchTransactionService.GetTransactionsByFicheReferenceId(
                httpClient: httpClient, 
                firmNumber: _httpClientService.FirmNumber, 
                periodNumber: _httpClientService.PeriodNumber, 
                ficheReferenceId: SalesFicheModel.ReferenceId, 
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
                        var item = Mapping.Mapper.Map<SalesTransactionModel>(transaction);
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
            var result = await _salesDispatchTransactionService.GetTransactionsByFicheReferenceId(
                httpClient: httpClient, 
                firmNumber: _httpClientService.FirmNumber, 
                periodNumber: _httpClientService.PeriodNumber, 
                ficheReferenceId: SalesFicheModel.ReferenceId, 
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
                        var item = Mapping.Mapper.Map<SalesTransactionModel>(transaction);
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

    private async Task ItemTappedAsync(SalesTransactionModel item)
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
                    SelectedSalesTransactions.Remove(SelectedSalesTransactions.FirstOrDefault(x => x.ReferenceId == selectedItem.ReferenceId));
                    SelectedProducts.Remove(SelectedProducts.FirstOrDefault(x => x.LineReferenceId == selectedItem.ReferenceId));
                    SelectedItems.Remove(selectedItem);
                }
                else
                {
                    Items.FirstOrDefault(x => x.ReferenceId == item.ReferenceId).IsSelected = true;

                    var basketItem = new ReturnSalesBasketModel
                    {
						LineReferenceId = item.ReferenceId,
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
                        Image = item.ImageData
                    };

                    SelectedProducts.Add(basketItem);

                    SelectedSalesTransactions.Add(selectedItem);
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

    private async Task LoadVariantItemsAsync()
    {
        try
        {
            _userDialogs.Loading("Loading Variants...");

			var httpClient = _httpClientService.GetOrCreateHttpClient();
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            
        }
    }

    private async Task LoadMoreVariantItemsAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            var httpClient = _httpClientService.GetOrCreateHttpClient();

        }
        catch (Exception ex)
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

    private async Task VariantTappedAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
        }
        catch (Exception ex)
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

    private async Task ConfirmVariantAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
        }
        catch (Exception ex)
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
            SelectedItems.Clear();
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
            var result = await _salesDispatchTransactionService.GetTransactionsByFicheReferenceId(
                httpClient: httpClient, 
                firmNumber: _httpClientService.FirmNumber, 
                periodNumber: _httpClientService.PeriodNumber, 
                ficheReferenceId: SalesFicheModel.ReferenceId, 
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
                        var item = Mapping.Mapper.Map<SalesTransactionModel>(transaction);
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

            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
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
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
        finally
        {
            IsBusy = false;
        }
    }
}