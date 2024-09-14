using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.ProductModule.ProductPanel.ViewModels;

public partial class ProductPanelViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly IProductPanelService _productPanelService;

    [ObservableProperty]
    public ProductPanelModel productPanelModel = new();

    public ProductPanelViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService, IProductPanelService productPanelService)
    {
        _userDialogs = userDialogs;
        _httpClientService = httpClientService;
        _productPanelService = productPanelService;

        Title = "Malzeme Paneli";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        ItemTappedCommand = new Command<ProductFiche>(async (productFiche) => await ItemTappedAsync(productFiche));
    }

    public Page CurrentPage { get; set; }

    public Command LoadItemsCommand { get; }
    public Command ItemTappedCommand { get; }

    public async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(500);

            await Task.WhenAll(GetInputProductQuantityAsync(), GetOutputProductQuantityAsync());

            await Task.WhenAll(GetLastProductsAsync(), GetLastWarehousesAsync(), GetLastFicheAsync());

            if (_userDialogs.IsHudShowing)
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

    private async Task ItemTappedAsync(ProductFiche productFiche)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(500);
            await GetLastTransactionsAsync(productFiche);
            CurrentPage.FindByName<BottomSheet>("ficheTransactionBottomSheet").State = BottomSheetState.HalfExpanded;

            if (_userDialogs.IsHudShowing)
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

    private async Task GetLastProductsAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _productPanelService.GetLastProducts(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                ProductPanelModel.LastProducts.Clear();
                foreach (var item in result.Data)
                    ProductPanelModel.LastProducts.Add(Mapping.Mapper.Map<ProductModel>(item));
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
    }

    private async Task GetLastWarehousesAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _productPanelService.GetLastWarehouses(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                ProductPanelModel.LastWarehouses.Clear();
                foreach (var item in result.Data)
                    ProductPanelModel.LastWarehouses.Add(Mapping.Mapper.Map<WarehouseModel>(item));
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
    }

    private async Task GetLastTransactionsAsync(ProductFiche productFiche)
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _productPanelService.GetLastTransactions(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, productFiche.ReferenceId);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                ProductPanelModel.LastTransactions.Clear();
                foreach (var item in result.Data)
                    ProductPanelModel.LastTransactions.Add(Mapping.Mapper.Map<ProductTransaction>(item));
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
    }

    private async Task GetLastFicheAsync()
    {


        try
        {

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _productPanelService.GetLastFiche(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                ProductPanelModel.LastProductFiche.Clear();
                foreach (var item in result.Data)
                    ProductPanelModel.LastProductFiche.Add(Mapping.Mapper.Map<ProductFiche>(item));
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
        finally
        {
        }
    }

    private async Task GetInputProductQuantityAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _productPanelService.GetInputProductQuantity(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var obj = Mapping.Mapper.Map<ProductPanelModel>(item);
                    ProductPanelModel.InputProductQuantity = obj.InputProductQuantity;
                }

            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
    }

    private async Task GetOutputProductQuantityAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _productPanelService.GetOutputProductQuantity(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var obj = Mapping.Mapper.Map<ProductPanelModel>(item);
                    ProductPanelModel.OutputProductQuantity = obj.OutputProductQuantity;
                }
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
    }
}
