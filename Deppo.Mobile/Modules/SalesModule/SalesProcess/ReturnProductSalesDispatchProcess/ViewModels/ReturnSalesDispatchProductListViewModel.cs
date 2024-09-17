using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesDispatchProcess.Views;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesDispatchProcess.ViewModels;
[QueryProperty(name: nameof(PurchaseFicheModel), queryId: nameof(PurchaseFicheModel))]
public partial class ReturnSalesDispatchProductListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly ISalesDispatchTransactionService _salesDispatchTransactionService;

    [ObservableProperty]
    private PurchaseFicheModel purchaseFicheModel = null!;

    public ReturnSalesDispatchProductListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, ISalesDispatchTransactionService salesDispatchTransactionService)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _salesDispatchTransactionService = salesDispatchTransactionService;


    }

    public ObservableCollection<SalesTransactionModel> Items { get; } = new();

    public ObservableCollection<SalesTransactionModel> SelectedSalesTransactions { get; } = new();


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
            var result = await _salesDispatchTransactionService.GetTransactionsByFicheReferenceId(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, PurchaseFicheModel.ReferenceId, string.Empty, 0, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var transaction in result.Data)
                    {
                        var item = Mapping.Mapper.Map<SalesTransaction>(transaction);
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
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _salesDispatchTransactionService.GetTransactionsByFicheReferenceId(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, PurchaseFicheModel.ReferenceId, string.Empty, Items.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var transaction in result.Data)
                    {

                        var item = Mapping.Mapper.Map<SalesTransactionModel>(transaction);
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

    private async Task ItemTappedAsync(SalesTransactionModel salesTransactionModel)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            var selectedItem = Items.FirstOrDefault(x => x.ProductReferenceId == salesTransactionModel.ProductReferenceId);
            if (selectedItem is not null)
            {
                if (selectedItem.IsSelected)
                {
                    Items.FirstOrDefault(x => x.ProductReferenceId == salesTransactionModel.ProductReferenceId).IsSelected = false;
                    SelectedSalesTransactions.Remove(SelectedSalesTransactions.FirstOrDefault(x => x.ProductReferenceId == selectedItem.ProductReferenceId));
                }
                else
                {
                    Items.FirstOrDefault(x => x.ProductReferenceId == salesTransactionModel.ProductReferenceId).IsSelected = true;



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

            await Shell.Current.GoToAsync($"{nameof(ReturnSalesDispatchBasketView)}", new Dictionary<string, object>
            {
                [nameof(SelectedSalesTransactions)] = SelectedSalesTransactions,
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