using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;
using Org.Apache.Http.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionPanel.ViewModels
{
    public partial class QuicklyProductionPanelAllFicheListViewModel : BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IQuicklyProductionPanelService _quicklyProductionPanelService;
        private readonly IUserDialogs _userDialogs;

        [ObservableProperty]
        public ProductionFiche selectedItem;

        public ObservableCollection<ProductionFiche> Items { get; } = new();
        public ObservableCollection<ProductionTransaction> Transactions { get; } = new();

        public QuicklyProductionPanelAllFicheListViewModel(IHttpClientService httpClientService, IQuicklyProductionPanelService quicklyProductionPanelService, IUserDialogs userDialogs)
        {
            _httpClientService = httpClientService;
            _quicklyProductionPanelService = quicklyProductionPanelService;
            _userDialogs = userDialogs;

            Title = "Hareketler";

            LoadItemsCommand = new Command(async () => await LoadItemsAsync());
            LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
            ItemTappedCommand = new Command<ProductionFiche>(async (item) => await ItemTappedAsync(item));
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
                var result = await _quicklyProductionPanelService.GetAllProductionFiches(httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, search: "", skip: 0, take: 20);
                if (result.IsSuccess && result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var productionFiche = Mapping.Mapper.Map<ProductionFiche>(item);
                        if (!Items.Any(x => x.ReferenceId == productionFiche.ReferenceId))
                        {
                            Items.Add(productionFiche);
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
                int skipCount = Items.Count; // Mevcut öğe sayısına göre atlama yap
                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _quicklyProductionPanelService.GetAllProductionFiches(httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, search: "", skip: skipCount, take: 20);
                if (result.IsSuccess && result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var productionFiche = Mapping.Mapper.Map<ProductionFiche>(item);
                        if (!Items.Any(x => x.ReferenceId == productionFiche.ReferenceId))
                        {
                            Items.Add(productionFiche);
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

        private async Task ItemTappedAsync(ProductionFiche productionFiche)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                SelectedItem = productionFiche;

                await LoadTransactionsAsync(productionFiche);
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

        private async Task LoadTransactionsAsync(ProductionFiche productionFiche)
        {
            try
            {
                _userDialogs.ShowLoading("Yükleniyor...");
                await Task.Delay(1000);

                Transactions.Clear();

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _quicklyProductionPanelService.GetProductionTransactions(
                    httpClient: httpClient,
                    firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber,
                    productionFiche.ReferenceId,
                    string.Empty,
                    skip: 0,
                    take: 20);

                if (result.IsSuccess)
                {
                    if (result.Data != null)
                    {
                        foreach (var item in result.Data)
                        {
                            var transaction = Mapping.Mapper.Map<ProductionTransaction>(item);
                            if (!Transactions.Any(t => t.ReferenceId == transaction.ReferenceId))
                            {
                                Transactions.Add(transaction);
                            }
                        }
                    }
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
                var result = await _quicklyProductionPanelService.GetProductionTransactions(
                    httpClient: httpClient,
                    firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber,
                    SelectedItem.ReferenceId,
                    string.Empty,
                    skip: Transactions.Count,
                    take: 20);

                if (result.IsSuccess)
                {
                    if (result.Data != null)
                    {
                        foreach (var item in result.Data)
                        {
                            var transaction = Mapping.Mapper.Map<ProductionTransaction>(item);
                            if (!Transactions.Any(t => t.ReferenceId == transaction.ReferenceId))
                            {
                                Transactions.Add(transaction);
                            }
                        }
                    }
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