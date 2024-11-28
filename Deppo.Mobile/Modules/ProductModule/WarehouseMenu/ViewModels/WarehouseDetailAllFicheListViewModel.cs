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

namespace Deppo.Mobile.Modules.ProductModule.WarehouseMenu.ViewModels
{
    [QueryProperty(name: nameof(WarehouseDetailModel), queryId: nameof(WarehouseDetailModel))]
    public partial class WarehouseDetailAllFicheListViewModel : BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IWarehouseDetailAllFicheListService _warehouseDetailAllFicheListService;
        private readonly IUserDialogs _userDialogs;

        [ObservableProperty]
        private WarehouseDetailModel warehouseDetailModel = null!;

        [ObservableProperty]
        public WarehouseFiche selectedItem;

        public ObservableCollection<WarehouseFiche> Items { get; } = new();

        public ObservableCollection<WarehouseTransaction> Transactions { get; } = new();

        public WarehouseDetailAllFicheListViewModel(IHttpClientService httpClientService, IWarehouseDetailAllFicheListService warehouseDetailAllFicheListService, IUserDialogs userDialogs)
        {
            _httpClientService = httpClientService;
            _warehouseDetailAllFicheListService = warehouseDetailAllFicheListService;
            _userDialogs = userDialogs;

            Title = "Hareketler";

            LoadItemsCommand = new Command(async () => await LoadItemsAsync());
            LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
            ItemTappedCommand = new Command<WarehouseFiche>(async (item) => await ItemTappedAsync(item));
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
                Items.Clear(); // İlk yüklemede mevcut öğeleri temizler
                await Task.Delay(1000);

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _warehouseDetailAllFicheListService.GetAllFiches(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, warehouseNumber: WarehouseDetailModel.Warehouse.Number, string.Empty, skip: 0, take: 20);

                if (result.IsSuccess && result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var warehouseFiche = Mapping.Mapper.Map<WarehouseFiche>(item);
                        Items.Add(warehouseFiche);
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
                var result = await _warehouseDetailAllFicheListService.GetAllFiches(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, warehouseNumber: WarehouseDetailModel.Warehouse.Number, search: "", skip: Transactions.Count, take: 20);

                if (result.IsSuccess && result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var warehouseFiche = Mapping.Mapper.Map<WarehouseFiche>(item);

                        // Aynı öğeyi tekrar eklememek için kontrol et
                        if (!Items.Any(x => x.ReferenceId == warehouseFiche.ReferenceId))
                        {
                            Items.Add(warehouseFiche);
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

        private async Task ItemTappedAsync(WarehouseFiche warehouseFiche)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                SelectedItem = warehouseFiche;

                await LoadTransactionsAsync(warehouseFiche);
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

        private async Task LoadTransactionsAsync(WarehouseFiche warehouseFiche)
        {
            try
            {
                _userDialogs.ShowLoading("Yükleniyor...");
                await Task.Delay(1000);

                Transactions.Clear(); // İlk yüklemede listeyi temizliyorsunuz

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _warehouseDetailAllFicheListService.GetTransactionsByFiche(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, ficheReferenceId: SelectedItem.ReferenceId, warehouseNumber: WarehouseDetailModel.Warehouse.Number, skip: Transactions.Count, take: 20);
                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var item in result.Data)
                    {
                        var transaction = Mapping.Mapper.Map<WarehouseTransaction>(item);

                        // Eğer Transaction zaten yoksa ekleyin
                        if (!Transactions.Any(x => x.ReferenceId == transaction.ReferenceId))
                        {
                            Transactions.Add(transaction);
                        }
                    }

                    // WarehouseFiche'yi listeye ekleyin eğer daha önce eklenmemişse
                    if (!Items.Any(x => x.ReferenceId == warehouseFiche.ReferenceId))
                    {
                        Items.Add(warehouseFiche);
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
                var result = await _warehouseDetailAllFicheListService.GetTransactionsByFiche(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, ficheReferenceId: SelectedItem.ReferenceId, warehouseNumber: WarehouseDetailModel.Warehouse.Number, skip: Transactions.Count, take: 20);
                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var item in result.Data)
                    {
                        var transaction = Mapping.Mapper.Map<WarehouseTransaction>(item);

                        // Eğer Transaction zaten yoksa ekleyin
                        if (!Transactions.Any(x => x.ReferenceId == transaction.ReferenceId))
                        {
                            Transactions.Add(transaction);
                        }
                    }
                }

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