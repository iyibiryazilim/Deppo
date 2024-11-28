using System;
using System.Collections.ObjectModel;
using Android.Database;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionPanel.ViewModels;

public partial class QuicklyProductionInputProductListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IQuicklyProductionPanelService _quicklyProductionPanelService;
    private readonly IUserDialogs _userDialogs;

    public QuicklyProductionInputProductListViewModel(
        IHttpClientService httpClientService,
        IQuicklyProductionPanelService quicklyProductionPanelService,
        IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _quicklyProductionPanelService = quicklyProductionPanelService;
        _userDialogs = userDialogs;

        Title = "Üretilen Malzeme Listesi";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<ProductModel>(async (product) => await ItemTappedAsync(product));
        CloseCommand = new Command(async () => await CloseAsync());
    }

    public Page CurrentPage { get; set; }
    public ObservableCollection<ProductModel> Items { get; } = new();
    public ObservableCollection<ProductionTransaction> Transactions { get; } = new();

    [ObservableProperty]
    private ProductModel? selectedProduct;

    public Command LoadItemsCommand { get; set; }
    public Command LoadMoreItemsCommand { get; set; }

    public Command ItemTappedCommand { get; }
    public Command CloseCommand { get; set; }

    public async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor...");

            Items.Clear(); // İlk yüklemede listeyi temizle
            await Task.Delay(500);

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _quicklyProductionPanelService.GetQuicklyProductionInputProductListAsync(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                search: "",
                skip: 0,
                take: 20
            );

            if (result.IsSuccess && result.Data is not null)
            {
                foreach (var item in result.Data)
                {
                    var data = Mapping.Mapper.Map<ProductModel>(item);

                    // Aynı veriyi tekrar eklememek için kontrol
                    if (!Items.Any(existingItem => existingItem.ReferenceId == data.ReferenceId))
                    {
                        Items.Add(data);
                    }
                }
            }

            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
        }
        catch (Exception)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert("Bir hata oluştu. Lütfen tekrar deneyiniz.");
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
            _userDialogs.ShowLoading("Load More Items...");

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _quicklyProductionPanelService.GetQuicklyProductionInputProductListAsync(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                search: "",
                skip: Items.Count, // Yüklenen veri miktarı kadar atla
                take: 20
            );

            if (result.IsSuccess && result.Data is not null)
            {
                foreach (var item in result.Data)
                {
                    var data = Mapping.Mapper.Map<ProductModel>(item);

                    // Aynı veriyi tekrar eklememek için kontrol
                    if (!Items.Any(existingItem => existingItem.ReferenceId == data.ReferenceId))
                    {
                        Items.Add(data);
                    }
                }
            }

            if (_userDialogs.IsHudShowing)
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

    private async Task ItemTappedAsync(ProductModel product)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            SelectedProduct = product;

            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);
            await GetLastCountingTransactionsAsync(SelectedProduct);

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

    private async Task GetLastCountingTransactionsAsync(ProductModel productModel)
    {
        try
        {
            Transactions.Clear();
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _quicklyProductionPanelService.GetInputTransactions(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                productReferenceId: productModel.ReferenceId
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<ProductionTransaction>(item);
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

    private async Task CloseAsync()
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
            if (Transactions.Count > 0)
            {
                Transactions.Clear();
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