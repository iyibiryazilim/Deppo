using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Sys.Service.Models;
using DevExpress.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.TransactionSchedulerModule.ViewModels;

public partial class TransactionSchedulerViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly ITransactionSchedulerService _transactionSchedulerService;

    [ObservableProperty]
    private DateTime selectedDate = DateTime.Now;

    [ObservableProperty]
    private BaseFiche selectedBaseFiche;

    public TransactionSchedulerViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, ITransactionSchedulerService transactionSchedulerService)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _transactionSchedulerService = transactionSchedulerService;

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        SelectedChangedCommand = new Command(async () => await SelectedChangedAsync());
        ItemTappedCommand = new Command<BaseFiche>(async (baseFiche) => await ItemTappedAsync(baseFiche));
        CloseCommand = new Command(async () => await CloseAsync());

        Title = "Takvim";
    }

    public Page CurrentPage { get; set; }

    public Command LoadItemsCommand { get; }

    public Command SelectedChangedCommand { get; }

    public Command ItemTappedCommand { get; }

    public Command CloseCommand { get; }

    public ObservableCollection<BaseFiche> Items { get; } = new();

    public ObservableCollection<BaseTransaction> BaseTransactionItems { get; } = new();

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

            var result = await _transactionSchedulerService.GetAllFiches(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                selectedDate: SelectedDate,
                search: string.Empty,
                skip: 0,
                take: 20
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        Items.Add(Mapping.Mapper.Map<BaseFiche>(item));
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

    public async Task SelectedChangedAsync()
    {
        await LoadItemsAsync();
    }

    private async Task ItemTappedAsync(BaseFiche baseFiche)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(500);
            await GetLastTransactionsAsync(baseFiche);
            CurrentPage.FindByName<BottomSheet>("ficheTransactionBottomSheet").State = BottomSheetState.HalfExpanded;

            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task GetLastTransactionsAsync(BaseFiche baseFiche)
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _transactionSchedulerService.GetLastTransactions(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, baseFiche.ReferenceId);

            SelectedBaseFiche = baseFiche;

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                BaseTransactionItems.Clear();
                foreach (var item in result.Data)
                    BaseTransactionItems.Add(Mapping.Mapper.Map<BaseTransaction>(item));
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
    }

    private async Task CloseAsync()
    {
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
            IsBusy = true;
        }
    }
}