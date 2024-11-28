using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductMenu.Views;
using Deppo.Mobile.Modules.ProductModule.ProductPanel.Views;
using Deppo.Mobile.Modules.ProductModule.WarehouseMenu.ViewModels;
using Deppo.Mobile.Modules.ProductModule.WarehouseMenu.Views;
using DevExpress.Maui.Controls;

namespace Deppo.Mobile.Modules.ProductModule.ProductPanel.ViewModels;

public partial class ProductPanelViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly IProductPanelService _productPanelService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    public ProductPanelModel productPanelModel = new();

    [ObservableProperty]
    private ProductModel? selectedProduct;

    [ObservableProperty]
    private ProductFiche selectedProductFiche;

    public ProductPanelViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService, IProductPanelService productPanelService, IServiceProvider serviceProvider)
    {
        _userDialogs = userDialogs;
        _httpClientService = httpClientService;
        _productPanelService = productPanelService;
        _serviceProvider = serviceProvider;

        Title = "Malzeme Paneli";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        ItemTappedCommand = new Command<ProductFiche>(async (productFiche) => await ItemTappedAsync(productFiche));
        WarehouseTappedCommand = new Command<WarehouseModel>(async (warehouseModel) => await WarehouseTappedAsync(warehouseModel));
        ProductTappedCommand = new Command<ProductModel>(async (productModel) => await ProductTappedAsync(productModel));
        ProductInputTappedCommand = new Command(async () => await ProductInputTappedAsync());
        ProductOutputTappedCommand = new Command(async () => await ProductOutputTappedAsync());
        ButtonAllTappedCommand = new Command(async () => await ButtonAllTappedAsync());
    }

    public Page CurrentPage { get; set; }

    public Command LoadItemsCommand { get; }
    public Command ItemTappedCommand { get; }

    public Command<WarehouseModel> WarehouseTappedCommand { get; }
    public Command<ProductModel> ProductTappedCommand { get; }
    public Command ProductInputTappedCommand { get; }
    public Command ProductOutputTappedCommand { get; }
    public Command ButtonAllTappedCommand { get; }

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

            SelectedProductFiche = productFiche;

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

    private async Task WarehouseTappedAsync(WarehouseModel warehouseModel)
    {
        if (warehouseModel is null)
            return;
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            WarehouseDetailModel warehouseDetailModel = new();
            var warehouseItem = Mapping.Mapper.Map<Warehouse>(warehouseModel);
            warehouseDetailModel.Warehouse = warehouseItem;

            await Shell.Current.GoToAsync($"{nameof(WarehouseDetailView)}", new Dictionary<string, object>
            {
                [nameof(WarehouseDetailModel)] = warehouseDetailModel
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

            var productItem = Mapping.Mapper.Map<Product>(productModel);

            productDetailModel.Product = productItem;

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

    private async Task ProductInputTappedAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(InputProductListView)}");
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

            await Shell.Current.GoToAsync($"{nameof(OutputProductListView)}");
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

    private async Task ButtonAllTappedAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            await Shell.Current.GoToAsync($"{nameof(AllFicheListView)}");
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