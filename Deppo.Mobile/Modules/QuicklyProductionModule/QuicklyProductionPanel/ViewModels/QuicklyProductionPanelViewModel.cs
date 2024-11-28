using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.QuicklyModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductMenu.Views;
using Deppo.Mobile.Modules.ProductModule.ProductPanel.Views;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionPanel.Views;
using Deppo.Mobile.Modules.SalesModule.SalesPanel.Views;
using DevExpress.Maui.Controls;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionPanel.ViewModels;

public partial class QuicklyProductionPanelViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IQuicklyProductionPanelService _quicklyProductionPanelService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    private QuicklyProductionPanelModel quicklyProductionPanelModel = new();

    public QuicklyProductionPanelViewModel(IHttpClientService httpClientService, IQuicklyProductionPanelService quicklyProductionPanelService, IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _quicklyProductionPanelService = quicklyProductionPanelService;
        _userDialogs = userDialogs;

        Title = "Hızlı Üretim Paneli";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        ItemTappedCommand = new Command<ProductionFiche>(async (productionFiche) => await ItemTappedAsync(productionFiche));
        ProductTappedCommand = new Command<ProductModel>(async (productModel) => await ProductTappedAsync(productModel));
        AllFicheTappedCommand = new Command(async () => await AllFicheTappedAsync());
        ProductInputTappedCommand = new Command(async () => await ProductInputTappedAsync());
        ProductOutputTappedCommand = new Command(async () => await ProductOutputTappedAsync());
    }

    public Page CurrentPage { get; set; }
    public Command LoadItemsCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command AllFicheTappedCommand { get; }

    public Command ProductInputTappedCommand { get; }
    public Command ProductOutputTappedCommand { get; }
    public Command ProductTappedCommand { get; }

    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            QuicklyProductionPanelModel.LastProducts.Clear();
            QuicklyProductionPanelModel.LastProductionFiche.Clear();

            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);

            await Task.WhenAll(
                GetOutProductCountAsync(),
                GetInProductCountAsync()
            ).ContinueWith(async _ =>
            {
                await GetLastProductsAsync();
                await GetLastProductionFichesAsync();
                //QuicklyProductionPanelModel.InProductCountTotalRate = (double)((double)QuicklyProductionPanelModel.InProductCount / (double)QuicklyProductionPanelModel.TotalProductCount);
                //QuicklyProductionPanelModel.OutProductCountTotalRate = (double)((double)QuicklyProductionPanelModel.OutProductCount / (double)QuicklyProductionPanelModel.TotalProductCount);
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

    private async Task ItemTappedAsync(ProductionFiche productionFiche)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);
            await GetLastProductionTransactionsAsync(productionFiche);

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

    private async Task GetLastProductionTransactionsAsync(ProductionFiche productionFiche)
    {
        try
        {
			QuicklyProductionPanelModel.LastProductionTransaction.Clear();
			var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _quicklyProductionPanelService.GetLastProductionTransactions(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                ficheReferenceId: productionFiche.ReferenceId
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<ProductionTransaction>(item);
                        QuicklyProductionPanelModel.LastProductionTransaction.Add(obj);
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

    private async Task GetOutProductCountAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _quicklyProductionPanelService.GetOutProductCount(
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
                        var obj = Mapping.Mapper.Map<QuicklyProductionPanelModel>(item);
                        QuicklyProductionPanelModel.OutProductCount = obj.OutProductCount;
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

    private async Task GetInProductCountAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _quicklyProductionPanelService.GetInProductCount(
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
                        var obj = Mapping.Mapper.Map<QuicklyProductionPanelModel>(item);
                        QuicklyProductionPanelModel.InProductCount = obj.InProductCount;
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
            var result = await _quicklyProductionPanelService.GetLastProducts(
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
                        QuicklyProductionPanelModel.LastProducts.Add(obj);
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

    private async Task GetLastProductionFichesAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _quicklyProductionPanelService.GetLastProductionFiches(
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
                        var obj = Mapping.Mapper.Map<ProductionFiche>(item);
                        QuicklyProductionPanelModel.LastProductionFiche.Add(obj);
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

    private async Task AllFicheTappedAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(QuicklyProductionPanelAllFicheListView)}");
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

    private async Task ProductInputTappedAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(QuicklyProductionInputProductListView)}");
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

    private async Task ProductOutputTappedAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(QuicklyProductionOutputProductListView)}");
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

    private async Task ProductTappedAsync(ProductModel productModel)
    {
        if (productModel is null)
            return;
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            ProductDetailModel productDetailModel = new();
            productDetailModel.Product = productModel;

            await Shell.Current.GoToAsync($"{nameof(ProductDetailView)}", new Dictionary<string, object>
            {
                [nameof(ProductDetailModel)] = productDetailModel
            });
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
}