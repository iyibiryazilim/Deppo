using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.CountingModule.CountingPanel.ViewModels;

public partial class CountingTransactionsListViewModel : BaseViewModel
{

    private readonly IHttpClientService _httpClientService;
    private readonly ICountingPanelService _countingPanelService;
    private readonly IUserDialogs _userDialogs;
    public ObservableCollection<CountingFiche> Items { get; } = new();

    public ObservableCollection<CountingTransaction> Transactions { get; } = new();
    public CountingTransactionsListViewModel(IHttpClientService httpClientService, ICountingPanelService countingPanelService, IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _countingPanelService = countingPanelService;
        _userDialogs = userDialogs;

        Title = "Sayım İrsaliye Listesi";

        BackCommand = new Command(async () => await BackAsync());
        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<CountingFiche>(async (fiche) => await ItemTappedAsync(fiche));
    }

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command BackCommand { get; }

    public Command ItemTappedCommand { get; }
    public Page CurrentPage { get; set; }



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

            var result = await _countingPanelService.GetCountingFiches(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, 0, 20);
            if (result.IsSuccess && result.Data is not null)
            {
                foreach (var item in result.Data)
                {
                    var mappedItem = Mapping.Mapper.Map<CountingFiche>(item);
                    // Eğer item zaten ekli değilse koleksiyona ekle
                    if (!Items.Any(existingItem => existingItem.ReferenceId == mappedItem.ReferenceId))
                    {
                        Items.Add(mappedItem);
                    }
                }
            }
            _userDialogs.HideHud();
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

    private async Task LoadMoreItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("Yükleniyor...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var result = await _countingPanelService.GetCountingFiches(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, Items.Count, 20);
            if (result.IsSuccess && result.Data is not null)
            {
                foreach (var item in result.Data)
                {
                    var mappedItem = Mapping.Mapper.Map<CountingFiche>(item);
                    // Eğer item zaten ekli değilse koleksiyona ekle
                    if (!Items.Any(existingItem => existingItem.ReferenceId == mappedItem.ReferenceId))
                    {
                        Items.Add(mappedItem);
                    }
                }
            }
            _userDialogs.HideHud();
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



    private async Task BackAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            if (Items.Count > 0)
            {
                Items.Clear();
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


    private async Task ItemTappedAsync(CountingFiche fiche)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;


            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);
            await GetLastCountingTransactionsAsync(fiche);

            CurrentPage.FindByName<BottomSheet>("ficheTransactionBottomSheet").State = BottomSheetState.HalfExpanded;

            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
        }
        catch (System.Exception)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert("Bir hata oluştu. Lütfen tekrar deneyiniz.", "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }
    private async Task GetLastCountingTransactionsAsync(CountingFiche fiche)
    {
        try
        {
            Transactions.Clear();
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _countingPanelService.GetAllCountingTransactions(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                ficheReferenceId: fiche.ReferenceId
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<CountingTransaction>(item);
                        Transactions.Add(obj);
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
    }



}
