using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.OutsourceModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
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

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourcePanel.ViewModels
{
    public partial class OutsourcePanelAllFicheListViewModel : BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IUserDialogs _userDialogs;
        private readonly IOutsourcePanelService _outsourcePanelService;

        [ObservableProperty]
        public OutsourceFiche selectedItem;

        public ObservableCollection<OutsourceFiche> Items { get; } = new();

        public ObservableCollection<OutsourceTransactionModel> Transactions { get; } = new();

        public OutsourcePanelAllFicheListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IOutsourcePanelService outsourcePanelService)
        {
            _httpClientService = httpClientService;
            _userDialogs = userDialogs;
            _outsourcePanelService = outsourcePanelService;

            Title = "Fason Hareketleri";

            LoadItemsCommand = new Command(async () => await LoadItemsAsync());
            LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
            BackCommand = new Command(async () => await BackAsync());
            TransactionsCloseCommand = new Command(async () => await TransactionsCloseAsync());
            LoadMoreTransactionsCommand = new Command(async () => await LoadMoreFicheTransactionsAsync());
            ItemTappedCommand = new Command<OutsourceFiche>(async (item) => await ItemTappedAsync(item));
            BackCommand = new Command(async () => await BackAsync());
        }

        public Command LoadItemsCommand { get; }
        public Command LoadMoreItemsCommand { get; }
        public Command TransactionsCloseCommand { get; }
        public Command LoadMoreTransactionsCommand { get; }

        public Command LoadTransactionCommand { get; }

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
                _userDialogs.ShowLoading("Yükleniyor...");
                await Task.Delay(1000);

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _outsourcePanelService.GetAllOutsourceFiches(
                    httpClient: httpClient,
                    firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber, search: string.Empty, skip: 0, take: 20
                );

                if (result.IsSuccess)
                {
                    if (result.Data is not null)
                    {
                        foreach (var item in result.Data)
                        {
                            var obj = Mapping.Mapper.Map<OutsourceFiche>(item);
                            Items.Add(obj);
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

                _userDialogs.Alert(ex.Message, "Hata", "Tamam");
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
                var result = await _outsourcePanelService.GetAllOutsourceFiches(
                    httpClient: httpClient,
                    firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber, search: string.Empty, skip: 0, take: 20
                );
                if (result.IsSuccess)
                {
                    if (result.Data is not null)
                    {
                        foreach (var item in result.Data)
                        {
                            var obj = Mapping.Mapper.Map<OutsourceFiche>(item);
                            Items.Add(obj);
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

        private async Task ItemTappedAsync(OutsourceFiche outsourceFiche)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                SelectedItem = outsourceFiche;

                await LoadTransactionsAsync(outsourceFiche);
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

        private async Task LoadTransactionsAsync(OutsourceFiche outsourceFiche)
        {
            try
            {
                _userDialogs.ShowLoading("Yükleniyor...");
                await Task.Delay(1000);

                Transactions.Clear();

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _outsourcePanelService.GetLastOutsourceTransactions(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, _httpClientService.PeriodNumber, ficheReferenceId: outsourceFiche.ReferenceId);
                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var item in result.Data)
                        Transactions.Add(Mapping.Mapper.Map<OutsourceTransactionModel>(item));
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
                var result = await _outsourcePanelService.GetLastOutsourceTransactions(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, _httpClientService.PeriodNumber, ficheReferenceId: SelectedItem.ReferenceId);
                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var item in result.Data)
                        Transactions.Add(Mapping.Mapper.Map<OutsourceTransactionModel>(item));
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

        private async Task TransactionsCloseAsync()
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                CurrentPage.FindByName<BottomSheet>("ficheTransactionsBottomSheet").State = BottomSheetState.Hidden;
            });
        }
    }
}