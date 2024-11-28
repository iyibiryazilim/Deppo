using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseDispatchProcess.Views;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseDispatchProcess.ViewModels;


[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(PurchaseSupplier), queryId: nameof(PurchaseSupplier))]
public partial class ReturnPurchaseDispatchListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly IPurchaseDispatchTransactionService _purchaseDispatchTransactionService;


    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    private PurchaseSupplier purchaseSupplier = null!;

    [ObservableProperty]
    private PurchaseFicheModel purchaseFicheModel = null!;

    public ReturnPurchaseDispatchListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs,IPurchaseDispatchTransactionService purchaseDispatchTransactionService)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _purchaseDispatchTransactionService = purchaseDispatchTransactionService;

        Title = "SatınAlma İrsaliyeleri";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<PurchaseFicheModel>(async (x) => await ItemTappedAsync(x));

        NextViewCommand = new Command(async () => await NextViewAsync());
        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
    }

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command NextViewCommand { get; }
    public Command PerformSearchCommand { get; }
    public Command PerformEmptySearchCommand { get; }

    public ObservableCollection<PurchaseFicheModel> Items { get; } = new();

    [ObservableProperty]
    public SearchBar searchText;

    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            Items.Clear();
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _purchaseDispatchTransactionService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, PurchaseSupplier.ReferenceId, SearchText.Text, 0, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var fiche in result.Data)
                    {

                        var item = Mapping.Mapper.Map<PurchaseFicheModel>(fiche);
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
            var result = await _purchaseDispatchTransactionService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, PurchaseSupplier.ReferenceId, SearchText.Text, Items.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var fiche in result.Data)
                    {

                        var item = Mapping.Mapper.Map<PurchaseFicheModel>(fiche);
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

    private async Task ItemTappedAsync(PurchaseFicheModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

			if (item == PurchaseFicheModel)
			{
				PurchaseFicheModel.IsSelected = false;
				PurchaseFicheModel = null;
			}
			else
			{
				if (PurchaseFicheModel != null)
				{
					PurchaseFicheModel.IsSelected = false;
				}

				PurchaseFicheModel = item;
				PurchaseFicheModel.IsSelected = true;
			}
        }
        catch (Exception ex)
        {
            if(_userDialogs.IsHudShowing)
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

            if (PurchaseFicheModel is not null)
            {
                await Shell.Current.GoToAsync($"{nameof(ReturnPurchaseDispatchProductListView)}", new Dictionary<string, object>
                {
                    [nameof(PurchaseFicheModel)] = PurchaseFicheModel,
                    [nameof(PurchaseSupplier)] = PurchaseSupplier,
                    [nameof(WarehouseModel)] = WarehouseModel
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
            var result = await _purchaseDispatchTransactionService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, PurchaseSupplier.ReferenceId, SearchText.Text, 0, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var fiche in result.Data)
                    {

                        var item = Mapping.Mapper.Map<PurchaseFicheModel>(fiche);
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

}
