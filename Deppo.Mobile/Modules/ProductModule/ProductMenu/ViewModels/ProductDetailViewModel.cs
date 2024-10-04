using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.CountingModels.BasketModels;
using Deppo.Mobile.Core.Models.CountingModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.Views;
using Deppo.Mobile.Modules.ProductModule.ProductMenu.Views;
using DevExpress.Maui.Controls;
using Deppo.Mobile.Core.Models.ActionModel;
using System.Collections.ObjectModel;
using Deppo.Mobile.Modules.ProductModule.Variant;
using Deppo.Mobile.Core.Models.VariantModels;
using Deppo.Mobile.Modules.SalesModule.SalesPanel.Views;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Modules.SalesModule.CustomerMenu.Views;

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels;

[QueryProperty(name: nameof(ProductDetailModel), queryId: nameof(ProductDetailModel))]
public partial class ProductDetailViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IProductTransactionService _productTransactionService;
    private readonly ICustomQueryService _customQueryService;
    private readonly IUserDialogs _userDialogs;
    private readonly IProductDetailService _productDetailService;

    [ObservableProperty]
    private ProductDetailModel productDetailModel = null!;

    public ObservableCollection<ProductActionModel> ProductActionModels { get; } = new();

    public Page CurrentPage { get; set; }

    public ProductDetailViewModel(IHttpClientService httpClientService, IProductTransactionService productTransactionService, ICustomQueryService customQueryService, IUserDialogs userDialogs, IProductDetailService productDetailService)
    {
        Title = "Ürün Detayı";
        _httpClientService = httpClientService;
        _productTransactionService = productTransactionService;
        _customQueryService = customQueryService;
        _userDialogs = userDialogs;
        _productDetailService = productDetailService;

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        InputQuantityTappedCommand = new Command(async () => await InputQuantityTappedAsync());
        OutputQuantityTappedCommand = new Command(async () => await OutputQuantityTappedAsync());
        ShowProcessBottomSheetCommand = new Command(async () => await ShowProcessBottomSheetAsync());
        ActionModelProcessTappedCommand = new Command(async () => await ActionModelProcessTappedAsync());
        ActionModelsTappedCommand = new Command<ProductActionModel>(async (model) => await ActionModelsTappedAsync(model));
        GetLastTransactionsCommand = new Command(async () => await GetLastTransactionsAsync());
        AllFicheTappedCommand = new Command(async () => await AllFicheTappedAsync());
        ItemTappedCommand = new Command<ProductFiche>(async (productFiche) => await ItemTappedAsync(productFiche));

        GetLastTransactionCommand = new Command<ProductFiche>(async (productFiche) => await GetLastTransactionAsync(productFiche));
    }

    #region Commands

    public Command LoadItemsCommand { get; }
    public Command InputQuantityTappedCommand { get; }
    public Command OutputQuantityTappedCommand { get; }

    public Command ShowProcessBottomSheetCommand { get; }

    public Command GoToWarehouseTotalView { get; }

    public Command GetLastTransactionsCommand { get; }

    public Command AllFicheTappedCommand { get; }

    public Command GetLastTransactionCommand { get; }

    //ActionModel

    //Üç Nokta
    public Command ActionModelProcessTappedCommand { get; }

    //Tıkladığımı Diğer Sayfaya Göndereceğim
    public Command ActionModelsTappedCommand { get; }

    public Command ItemTappedCommand { get; }

    #endregion Commands

    private async Task LoadItemsAsync()
    {
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            _userDialogs.Loading("Loading Items...");
            await Task.Delay(1000);

            await Task.WhenAll(GetInputOutputQuantityAsync(httpClient), GetLastTransactionsAsync());

            _userDialogs.HideHud();
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(message: ex.Message, title: "Hata");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task GetInputOutputQuantityAsync(HttpClient httpClient)
    {
        try
        {
            var query = @$"SELECT
                    [InputQuantity] = (SELECT ISNULL(COUNT(DISTINCT STOCKREF), 0) FROM LG_{_httpClientService.FirmNumber.ToString().PadLeft(3, '0')}_{_httpClientService.PeriodNumber.ToString().PadLeft(2, '0')}_STLINE WHERE IOCODE IN(1, 2) AND STOCKREF = {ProductDetailModel.Product.ReferenceId}),
                    [OutputQuantity] = (SELECT ISNULL(COUNT(DISTINCT STOCKREF), 0) FROM LG_{_httpClientService.FirmNumber.ToString().PadLeft(3, '0')}_{_httpClientService.PeriodNumber.ToString().PadLeft(2, '0')}_STLINE WHERE IOCODE IN(3, 4) AND STOCKREF = {ProductDetailModel.Product.ReferenceId})";

            var result = await _customQueryService.GetObjectAsync(httpClient, query);

            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;
                var obj = Mapping.Mapper.Map<ProductDetailModel>(result.Data);
                ProductDetailModel.InputQuantity = obj.InputQuantity;
                ProductDetailModel.OutputQuantity = obj.OutputQuantity;
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(message: ex.Message, title: "Hata");
        }
    }

    private async Task GetLastTransactionsAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _productDetailService.GetLastFichesByProduct(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, ProductDetailModel.Product.ReferenceId);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                ProductDetailModel.Transactions.Clear();
                foreach (var item in result.Data)
                    ProductDetailModel.Transactions.Add(Mapping.Mapper.Map<ProductFiche>(item));
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

    private async Task InputQuantityTappedAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            await Task.Delay(300);
            await Shell.Current.GoToAsync($"{nameof(ProductInputTransactionView)}", new Dictionary<string, object>
            {
                ["Product"] = ProductDetailModel.Product
            });
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(message: ex.Message, title: "Hata");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task OutputQuantityTappedAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            await Task.Delay(300);
            await Shell.Current.GoToAsync($"{nameof(ProductOutputTransactionView)}", new Dictionary<string, object>
            {
                ["Product"] = ProductDetailModel.Product
            });
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(message: ex.Message, title: "Hata");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ShowProcessBottomSheetAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            CurrentPage.FindByName<BottomSheet>("processBottomSheet").State = BottomSheetState.HalfExpanded;
        }
        catch (System.Exception)
        {
            throw;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task GoToWarehouseTotalViewAsync()
    {
        try
        {
            //await Shell.Current.GoToAsync($"{nameof(WarehouseCountingFormView)}", new Dictionary<string, object>
            //{
            //    [nameof(LocationModel)] = LocationModel,
            //    [nameof(WarehouseCountingWarehouseModel)] = WarehouseCountingWarehouseModel,
            //    [nameof(WarehouseCountingBasketModel)] = SelectedItems
            //});
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
        }
    }

    private async Task ActionModelProcessTappedAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(500);
            await LoadActionModelsAsync();

            CurrentPage.FindByName<BottomSheet>("processBottomSheet").State = BottomSheetState.HalfExpanded;

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

    private async Task LoadActionModelsAsync()
    {
        try
        {
            IsBusy = true;
            ProductActionModels.Clear();
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            _userDialogs.Loading("Loading Items...");
            await Task.Delay(1000);

            ProductActionModels.Add(new ProductActionModel
            {
                ActionName = "Ambar Toplamları",
                ActionUrl = $"{nameof(ProductDetailWarehouseTotalListView)}"
            });
            if (ProductDetailModel.Product.IsVariant)
            {
                ProductActionModels.Add(new ProductActionModel
                {
                    ActionName = "Varyantları",
                    ActionUrl = $"{nameof(ProductVariantListViewModel)}"
                });
            }

            _userDialogs.HideHud();
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(message: ex.Message, title: "Hata");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ActionModelsTappedAsync(ProductActionModel model)
    {
        try
        {
            IsBusy = true;
            await Shell.Current.GoToAsync(model.ActionUrl);
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(message: ex.Message, title: "Hata");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task AllFicheTappedAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(ProductDetailAllFicheListView)}", new Dictionary<string, object> { {
                nameof(ProductDetailModel), ProductDetailModel
                }
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

    private async Task GetLastTransactionAsync(ProductFiche productFiche)
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _productDetailService.GetLastTransaction(httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                productReferenceId: ProductDetailModel.Product.ReferenceId,
                ficheReferenceId: productFiche.ReferenceId);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;
                ProductDetailModel.LastTransactions.Clear();

                foreach (var item in result.Data)
                    ProductDetailModel.LastTransactions.Add(Mapping.Mapper.Map<ProductTransaction>(item));
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

    private async Task ItemTappedAsync(ProductFiche productFiche)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(500);
            await GetLastTransactionAsync(productFiche);
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
}