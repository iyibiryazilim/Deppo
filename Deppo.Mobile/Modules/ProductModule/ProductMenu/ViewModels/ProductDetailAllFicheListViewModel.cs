using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
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

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels
{
    [QueryProperty(name: nameof(ProductDetailModel), queryId: nameof(ProductDetailModel))]
    public partial class ProductDetailAllFicheListViewModel : BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IProductDetailAllFichesService _productDetailAllFichesService;
        private readonly IUserDialogs _userDialogs;

        [ObservableProperty]
        private ProductDetailModel productDetailModel = null!;

        [ObservableProperty]
        public ProductFiche selectedItem;

        public ObservableCollection<ProductFiche> Items { get; } = new();

        public ObservableCollection<ProductTransaction> Transactions { get; } = new();

        public ProductDetailAllFicheListViewModel(IHttpClientService httpClientService, IProductDetailAllFichesService productDetailAllFichesService, IUserDialogs userDialogs)
        {
            _httpClientService = httpClientService;
            _productDetailAllFichesService = productDetailAllFichesService;
            _userDialogs = userDialogs;

            Title = "Malzeme Hareketleri";

            LoadItemsCommand = new Command(async () => await LoadItemsAsync());
            LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
            ItemTappedCommand = new Command<ProductFiche>(async (item) => await ItemTappedAsync(item));
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
                var result = await _productDetailAllFichesService.GetAllFiches(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, productRefernceId: ProductDetailModel.Product.ReferenceId, search: "", skip: 0, take: 20);

                if (result.IsSuccess && result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var productFiche = Mapping.Mapper.Map<ProductFiche>(item);

                        // Aynı öğeyi tekrar eklememek için kontrol et
                        if (!Items.Any(x => x.ReferenceId == productFiche.ReferenceId))
                        {
                            Items.Add(productFiche);
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
                var result = await _productDetailAllFichesService.GetAllFiches(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, productRefernceId: ProductDetailModel.Product.ReferenceId, search: "", skip: Transactions.Count, take: 20); // skip, zaten yüklenen öğeler kadar olmalı

                if (result.IsSuccess && result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var productFiche = Mapping.Mapper.Map<ProductFiche>(item);

                        // Aynı öğeyi tekrar eklememek için kontrol et
                        if (!Items.Any(x => x.ReferenceId == productFiche.ReferenceId))
                        {
                            Items.Add(productFiche);
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

        private async Task ItemTappedAsync(ProductFiche productFiche)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                SelectedItem = productFiche;

                await LoadTransactionsAsync(productFiche);
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

        private async Task LoadTransactionsAsync(ProductFiche productFiche)
        {
            try
            {
                _userDialogs.ShowLoading("Yükleniyor...");
                await Task.Delay(1000);

                Transactions.Clear();

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _productDetailAllFichesService.GetTransactionsByFiche(
                    httpClient: httpClient,
                    firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber,
                    ficheReferenceId: SelectedItem.ReferenceId,
                    productRefrenceId: ProductDetailModel.Product.ReferenceId,
                    skip: Transactions.Count,
                    take: 20
                );

                if (result.IsSuccess && result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var transaction = Mapping.Mapper.Map<ProductTransaction>(item);

                        // Aynı `ReferenceId`'ye sahip bir transaction varsa ekleme
                        if (!Transactions.Any(x => x.ReferenceId == transaction.ReferenceId))
                        {
                            Transactions.Add(transaction);
                        }
                    }

                    // Aynı `ReferenceId`'ye sahip bir `ProductFiche` yoksa ekle
                    if (!Items.Any(x => x.ReferenceId == productFiche.ReferenceId))
                    {
                        Items.Add(productFiche);
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
                var result = await _productDetailAllFichesService.GetTransactionsByFiche(
                    httpClient: httpClient,
                    firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber,
                    ficheReferenceId: SelectedItem.ReferenceId,
                    productRefrenceId: ProductDetailModel.Product.ReferenceId,
                    skip: Transactions.Count,
                    take: 20
                );

                if (result.IsSuccess && result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var transaction = Mapping.Mapper.Map<ProductTransaction>(item);

                        // Aynı `ReferenceId`'ye sahip bir transaction varsa ekleme
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