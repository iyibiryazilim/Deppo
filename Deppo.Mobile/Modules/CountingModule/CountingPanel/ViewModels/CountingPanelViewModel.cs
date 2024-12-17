using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.CountingModels;
using Deppo.Mobile.Core.Models.CountingModels.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.CountingModule.CountingPanel.Views;
using Deppo.Mobile.Modules.CountingModule.CountingProcess.ProductCountingProcess.Views;
using Deppo.Mobile.Modules.ProductModule.ProductMenu.Views;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;
using DevExpress.Maui.Controls;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.CountingModule.CountingPanel.ViewModels;

public partial class CountingPanelViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ICountingPanelService _countingPanelService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    CountingPanelModel _countingPanelModel = new();


    [ObservableProperty]
    private ProductModel? selectedProduct;



    public CountingPanelViewModel(IHttpClientService httpClientService, ICountingPanelService countingPanelService, IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _countingPanelService = countingPanelService;
        _userDialogs = userDialogs;

        Title = "Sayım Paneli";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        ItemTappedCommand = new Command<CountingFiche>(async (fiche) => await ItemTappedAsync(fiche));
        InCountingReferenceProductCommand = new Command(async () => await InCountingReferenceProductAsync());
        OutCountingReferenceProductCommand = new Command(async () => await OutCountingReferenceProductAsync());
        ProductTappedCommand = new Command<ProductModel>(async (product) => await ProducTappedAsync(product));
        GetAllTransactionsCommand = new Command(async () => await GetAllTransactionsAsync());
    }

    public Page CurrentPage { get; set; }
    public Command LoadItemsCommand { get; }
    public Command ItemTappedCommand { get; }

    public Command InCountingReferenceProductCommand { get; }
    public Command OutCountingReferenceProductCommand { get; }

    public Command ProductTappedCommand { get; }

    public Command GetAllTransactionsCommand { get; }


    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            CountingPanelModel.LastProducts.Clear();
            CountingPanelModel.LastCountingFiche.Clear();

            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);

            await Task.WhenAll(
                GetCountingOutProductCountAsync(),
                GetCountingInProductCountAsync(),
                GetCountingTotalProductCountAsync()
            ).ContinueWith(async _ =>
            {
                await GetLastProductsAsync();
                await GetLastCountingFichesAsync();
                CountingPanelModel.InProductCountTotalRate = (double)((double)CountingPanelModel.InProductCount / (double)CountingPanelModel.TotalProductCount);
                CountingPanelModel.OutProductCountTotalRate = (double)((double)CountingPanelModel.OutProductCount / (double)CountingPanelModel.TotalProductCount);
            });

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

    private async Task ItemTappedAsync(CountingFiche countingFiche)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);
            await GetLastCountingTransactionsAsync(countingFiche);

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



    private async Task ProducTappedAsync(ProductModel product)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;


            ProductDetailModel productDetailModel = new();

            var obj = Mapping.Mapper.Map<Product>(product);
            productDetailModel.Product = obj;



            await Shell.Current.GoToAsync($"{nameof(ProductDetailView)}", new Dictionary<string, object>
            {
                [nameof(ProductDetailModel)] = productDetailModel
            });


        }
        catch (Exception ex)
        {
            _userDialogs.Alert(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task GetLastCountingTransactionsAsync(CountingFiche countingFiche)
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _countingPanelService.GetLastCountingTransactions(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                ficheReferenceId: countingFiche.ReferenceId
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    // Önce listeyi temizleyin
                    CountingPanelModel.LastCountingTransaction.Clear();

                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<CountingTransaction>(item);
                        CountingPanelModel.LastCountingTransaction.Add(obj);
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


    private async Task GetCountingOutProductCountAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _countingPanelService.GetCountingOutProductCount(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<CountingPanelModel>(item);
                        CountingPanelModel.OutProductCount = obj.OutProductCount;
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

    private async Task GetCountingInProductCountAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _countingPanelService.GetCountingInProductCount(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<CountingPanelModel>(item);
                        CountingPanelModel.InProductCount = obj.InProductCount;
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

    private async Task GetCountingTotalProductCountAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _countingPanelService.GetCountingTotalProductCount(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<CountingPanelModel>(item);
                        CountingPanelModel.TotalProductCount = obj.TotalProductCount;
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

    private async Task GetLastProductsAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _countingPanelService.GetLastProducts(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<ProductModel>(item);
                        CountingPanelModel.LastProducts.Add(obj);
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

    private async Task GetLastCountingFichesAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _countingPanelService.GetLastCountingFiches(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                externalDb: _httpClientService.ExternalDatabase
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<CountingFiche>(item);
                        CountingPanelModel.LastCountingFiche.Add(obj);
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

    private async Task InCountingReferenceProductAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            await Shell.Current.GoToAsync($"{nameof(CountingInputReferenceProductListView)}");
        }
        catch (Exception ex)
        {
            _userDialogs.Alert(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }


    private async Task OutCountingReferenceProductAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            await Shell.Current.GoToAsync($"{nameof(CountingOutputReferenceProductListView)}");
        }
        catch (Exception ex)
        {
            _userDialogs.Alert(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task GetAllTransactionsAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            await Shell.Current.GoToAsync($"{nameof(CountingTransactionsListView)}");
        }
        catch (Exception ex)
        {
            _userDialogs.Alert(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }


}
