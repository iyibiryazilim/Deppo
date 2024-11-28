using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchasePanel.ViewModels
{
    public partial class PurchasePanelAllFicheListViewModel : BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IPurchasePanelService _purchasePanelService;
        private readonly IUserDialogs _userDialogs;

        [ObservableProperty]
        public PurchaseFiche selectedItem;

        public ObservableCollection<PurchaseFiche> Items { get; } = new();

        public ObservableCollection<PurchaseTransactionModel> Transactions { get; } = new();

        public PurchasePanelAllFicheListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IPurchasePanelService purchasePanelService)
        {
            _httpClientService = httpClientService;
            _userDialogs = userDialogs;
            _purchasePanelService = purchasePanelService;

            Title = "Satınalma İrsaliyeleri";

            LoadItemsCommand = new Command(async () => await LoadItemsAsync());
            LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
            ItemTappedCommand = new Command<PurchaseFiche>(async (item) => await ItemTappedAsync(item));
            LoadMoreTransactionsCommand = new Command(async () => await LoadMoreFicheTransactionsAsync());
            BackCommand = new Command(async () => await BackAsync());
        }

        public Command LoadItemsCommand { get; }
        public Command LoadMoreItemsCommand { get; }
        public Command ItemTappedCommand { get; }
        public Command NextViewCommand { get; }

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
                var result = await _purchasePanelService.GetAllFiche(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, string.Empty, 0, 20);
                if (result.IsSuccess)
                {
                    if (result.Data is not null)
                    {
                        foreach (var item in result.Data)
                        {
                            Items.Add(Mapping.Mapper.Map<PurchaseFiche>(item));
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
                var result = await _purchasePanelService.GetAllFiche(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, string.Empty, Items.Count, 20);
                if (result.IsSuccess)
                {
                    if (result.Data is not null)
                    {
                        foreach (var item in result.Data)
                        {
                            Items.Add(Mapping.Mapper.Map<PurchaseFiche>(item));
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

        private async Task ItemTappedAsync(PurchaseFiche purchaseFiche)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                SelectedItem = purchaseFiche;

                await LoadTransactionsAsync(purchaseFiche);
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

        private async Task LoadTransactionsAsync(PurchaseFiche purchaseFiche)
        {
            try
            {
                _userDialogs.ShowLoading("Yükleniyor...");
                await Task.Delay(1000);

                Transactions.Clear();

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _purchasePanelService.GetFicheTransactions(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, purchaseFiche.ReferenceId, string.Empty, 0, 20);
                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var item in result.Data)
                        Transactions.Add(Mapping.Mapper.Map<PurchaseTransactionModel>(item));
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
                var result = await _purchasePanelService.GetFicheTransactions(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SelectedItem.ReferenceId, string.Empty, Transactions.Count, 20);
                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var item in result.Data)
                        Transactions.Add(Mapping.Mapper.Map<PurchaseTransactionModel>(item));
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