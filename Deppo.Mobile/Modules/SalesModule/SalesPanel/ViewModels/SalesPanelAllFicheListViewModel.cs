using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesPanel.ViewModels
{
    public partial class SalesPanelAllFicheListViewModel : BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly ISalesPanelService _salesPanelService;
        private readonly IUserDialogs _userDialogs;


        [ObservableProperty]
        public SalesFiche selectedItem;
        public ObservableCollection<SalesFiche> Items { get; } = new();

        public ObservableCollection<SalesTransactionModel> Transactions { get; } = new();

        public SalesPanelAllFicheListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, ISalesPanelService salesPanelService)
        {
            _httpClientService = httpClientService;
            _salesPanelService = salesPanelService;
            _userDialogs = userDialogs;

            Title = "Satış İrsaliyeleri";

            LoadItemsCommand = new Command(async () => await LoadItemsAsync());
            LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
            ItemTappedCommand = new Command<SalesFiche>(async (item) => await ItemTappedAsync(item));
            TransactionsCloseCommand = new Command(async () => await TransactionsCloseAsync());
            LoadMoreTransactionsCommand = new Command(async () => await LoadMoreFicheTransactionsAsync());
            BackCommand = new Command(async () => await BackAsync());

        }

        public Command LoadItemsCommand { get; }
        public Command LoadMoreItemsCommand { get; }
        public Command ItemTappedCommand { get; }
        public Command NextViewCommand { get; }
        public Command TransactionsCloseCommand { get; }
        public Command LoadMoreTransactionsCommand { get; }
        public Command BackCommand { get; }

        public Page CurrentPage { get; set; }

        private async Task LoadItemsAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                _userDialogs.ShowLoading("Loading...");
                Items.Clear();
                await Task.Delay(1000);
                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _salesPanelService.GetAllFiche(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, string.Empty, 0, 20);
                if (result.IsSuccess)
                {
                    if (result.Data is not null)
                    {
                        foreach (var item in result.Data)
                        {
                            Items.Add(Mapping.Mapper.Map<SalesFiche>(item));
                        }

                    }
                }

                _userDialogs.HideHud();
            }
            catch (Exception ex)
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

                _userDialogs.Alert(ex.Message);
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
                var result = await _salesPanelService.GetAllFiche(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, string.Empty, Items.Count, 20);
                if (result.IsSuccess)
                {
                    if (result.Data is not null)
                    {
                        foreach (var item in result.Data)
                        {
                            Items.Add(Mapping.Mapper.Map<SalesFiche>(item));
                        }

                    }
                }

                _userDialogs.HideHud();
            }
            catch (Exception ex)
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

                _userDialogs.Alert(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ItemTappedAsync(SalesFiche salesFiche)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                SelectedItem = salesFiche;

                await LoadTransactionsAsync(salesFiche);
                CurrentPage.FindByName<BottomSheet>("ficheTransactionsBottomSheet").State = BottomSheetState.HalfExpanded;


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

        private async Task TransactionsCloseAsync()
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                CurrentPage.FindByName<BottomSheet>("ficheTransactionsBottomSheet").State = BottomSheetState.Hidden;
            });
        }
        private async Task LoadTransactionsAsync(SalesFiche salesFiche)
        {
            try
            {
                _userDialogs.ShowLoading("Yükleniyor...");
                await Task.Delay(1000);

                Transactions.Clear();

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _salesPanelService.GetFicheTransactions(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, salesFiche.ReferenceId, string.Empty, 0, 20);
                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var item in result.Data)
                        Transactions.Add(Mapping.Mapper.Map<SalesTransactionModel>(item));
                }

                _userDialogs.HideHud();
            }
            catch (Exception ex)
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

                _userDialogs.Alert(ex.Message, "Hata", "Tamam");
            }
        }

        private async Task LoadMoreFicheTransactionsAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _salesPanelService.GetFicheTransactions(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SelectedItem.ReferenceId, string.Empty, Transactions.Count, 20);
                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var item in result.Data)
                        Transactions.Add(Mapping.Mapper.Map<SalesTransactionModel>(item));
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
    }
}
