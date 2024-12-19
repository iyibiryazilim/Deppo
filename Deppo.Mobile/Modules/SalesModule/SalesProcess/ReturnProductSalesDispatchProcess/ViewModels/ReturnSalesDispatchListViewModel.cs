using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesDispatchProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesProcess.ViewModels;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesDispatchProcess.ViewModels;


[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(SalesCustomer), queryId: nameof(SalesCustomer))]
public partial class ReturnSalesDispatchListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly ISalesDispatchTransactionService _salesDispatchTransactionService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    private SalesCustomer salesCustomer = null!;

    [ObservableProperty]
    private SalesFicheModel salesFicheModel = null!;

    public ReturnSalesDispatchListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, ISalesDispatchTransactionService salesDispatchTransactionService, IServiceProvider serviceProvider)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _salesDispatchTransactionService = salesDispatchTransactionService;
        _serviceProvider = serviceProvider;

        Title = "İrsaliye Listesi";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
        ItemTappedCommand = new Command<SalesFicheModel>(async (x) => await ItemTappedAsync(x));
		BackCommand = new Command(async () => await BackAsync());
		NextViewCommand = new Command(async () => await NextViewAsync());
    }

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command PerformSearchCommand { get; }
    public Command PerformEmptySearchCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command NextViewCommand { get; }
    public Command BackCommand { get; }

    public ObservableCollection<SalesFicheModel> Items { get; } = new();

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
            var result = await _salesDispatchTransactionService.GetObjects(
                httpClient: httpClient, 
                firmNumber: _httpClientService.FirmNumber, 
                periodNumber: _httpClientService.PeriodNumber, 
                warehouseNumber: WarehouseModel.Number, 
                customerReferenceId: SalesCustomer.ReferenceId, 
                search: SearchText.Text, 
                skip: 0, 
                take: 20,
                externalDb: _httpClientService.ExternalDatabase
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var fiche in result.Data)
                    {
                        var item = Mapping.Mapper.Map<SalesFicheModel>(fiche);
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

        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor...");

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _salesDispatchTransactionService.GetObjects(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber, 
                warehouseNumber: WarehouseModel.Number, 
                customerReferenceId: SalesCustomer.ReferenceId, 
                search: SearchText.Text,
                skip: Items.Count, 
                take: 20,
                externalDb: _httpClientService.ExternalDatabase);

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var fiche in result.Data)
                    {
                        var item = Mapping.Mapper.Map<SalesFicheModel>(fiche);
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

    private async Task ItemTappedAsync(SalesFicheModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (item == SalesFicheModel)
            {
                SalesFicheModel.IsSelected = false;
                SalesFicheModel = null;
            }
            else
            {
                if (SalesFicheModel != null)
                {
                    SalesFicheModel.IsSelected = false;
                }

                SalesFicheModel = item;
                SalesFicheModel.IsSelected = true;
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            await _userDialogs.AlertAsync(ex.Message);
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

            if (SalesFicheModel is not null)
            {
                await Shell.Current.GoToAsync($"{nameof(ReturnSalesDispatchProductListView)}", new Dictionary<string, object>
                {
                    [nameof(SalesFicheModel)] = SalesFicheModel,
                    [nameof(WarehouseModel)] = WarehouseModel,
                    [nameof(SalesCustomer)] = SalesCustomer
                });
            }
        }
        catch (Exception ex)
        {
            _userDialogs.Alert(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
    public async Task LoadPageAsync()
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
            var result = await _salesDispatchTransactionService.GetObjects(
                httpClient: httpClient, 
                firmNumber: _httpClientService.FirmNumber, 
                periodNumber: _httpClientService.PeriodNumber,
                warehouseNumber: WarehouseModel.Number, 
                customerReferenceId: SalesCustomer.ReferenceId,
                search: SearchText.Text, 
                skip: 0, 
                take: 20,
                externalDb: _httpClientService.ExternalDatabase);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var fiche in result.Data)
                    {
                        var item = Mapping.Mapper.Map<SalesFicheModel>(fiche);
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

            if(SalesFicheModel is not null)
            {
                SalesFicheModel.IsSelected = false;
                SalesFicheModel = null;
            }

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