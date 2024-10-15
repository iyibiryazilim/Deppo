using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.OutsourceModels;
using Deppo.Mobile.Core.Models.SalesModels;
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

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceMenu.ViewModels
{
    [QueryProperty(name: nameof(OutsourceDetailModel), queryId: nameof(OutsourceDetailModel))]
    public partial class OutsourceDetailAllFichesViewModel : BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IOutsourceDetailAllFichesService _outsourceDetailAllFichesService;
        private readonly IUserDialogs _userDialogs;

        [ObservableProperty]
        private OutsourceDetailModel outsourceDetailModel = null!;

        [ObservableProperty]
        public OutsourceFiche selectedItem;

        public Page CurrentPage { get; set; }

        public ObservableCollection<OutsourceFiche> Items { get; } = new();

        public ObservableCollection<OutsourceTransactionModel> Transactions { get; } = new();

        public OutsourceDetailAllFichesViewModel(IHttpClientService httpClientService, IOutsourceDetailAllFichesService outsourceDetailAllFichesService, IUserDialogs userDialogs)
        {
            _httpClientService = httpClientService;
            _outsourceDetailAllFichesService = outsourceDetailAllFichesService;
            _userDialogs = userDialogs;

            Title = "Fiş Listesi";

            LoadItemsCommand = new Command(async () => await LoadItemsAsync());
            LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
            ItemTappedCommand = new Command<OutsourceFiche>(async (item) => await ItemTappedAsync(item));
            LoadMoreTransactionsCommand = new Command(async () => await LoadMoreFicheTransactionsAsync());
            BackCommand = new Command(async () => await BackAsync());
        }

        public Command LoadItemsCommand { get; }
        public Command LoadMoreItemsCommand { get; }
        public Command ItemTappedCommand { get; }
        public Command NextViewCommand { get; }
        public Command LoadMoreTransactionsCommand { get; }
        public Command BackCommand { get; }

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
                var result = await _outsourceDetailAllFichesService.GetAllFiches(
                    httpClient: httpClient,
                    firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber,
                    customerReferenceId: OutsourceDetailModel.Outsource.ReferenceId,
                    search: "",
                    skip: 0,
                    take: 20);

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var item in result.Data)
                    {
                        Items.Add(Mapping.Mapper.Map<OutsourceFiche>(item));
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

                _userDialogs.ShowLoading("Loading More...");

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _outsourceDetailAllFichesService.GetAllFiches(
                    httpClient: httpClient,
                    firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber,
                    customerReferenceId: OutsourceDetailModel.Outsource.ReferenceId,
                    search: "",
                    skip: Items.Count,
                    take: 20);

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var item in result.Data)
                    {
                        Items.Add(Mapping.Mapper.Map<OutsourceFiche>(item));
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

        private async Task ItemTappedAsync(OutsourceFiche outsourceFiche)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                SelectedItem = outsourceFiche;

                _userDialogs.ShowLoading("Yükleniyor...");
                await LoadTransactionsAsync(outsourceFiche);
                await Task.Delay(500);
                CurrentPage.FindByName<BottomSheet>("ficheTransactionsBottomSheet").State = BottomSheetState.HalfExpanded;
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

        private async Task LoadTransactionsAsync(OutsourceFiche outsourceFiche)
        {
            try
            {
                Transactions.Clear();

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _outsourceDetailAllFichesService.GetTransactionsByFiche(httpClient: httpClient,
                    firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber,
                    ficheReferenceId: outsourceFiche.ReferenceId,
                    skip: Transactions.Count,
                    take: 20);

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
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

                _userDialogs.Alert(ex.Message, "Hata", "Tamam");
            }
        }

        private async Task LoadMoreFicheTransactionsAsync()
        {
            if (Transactions.Count < 18)
                return;
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                _userDialogs.ShowLoading("Yükleniyor...");
                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _outsourceDetailAllFichesService.GetTransactionsByFiche(
                    httpClient: httpClient,
                    firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber,
                    ficheReferenceId: SelectedItem.ReferenceId,
                    skip: Transactions.Count,
                    take: 20);

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var item in result.Data)
                        Transactions.Add(Mapping.Mapper.Map<OutsourceTransactionModel>(item));
                }

                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();
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