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

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels;

[QueryProperty(name: nameof(ProductDetailModel), queryId: nameof(ProductDetailModel))]
public partial class ProductDetailViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IProductTransactionService _productTransactionService;
    private readonly ICustomQueryService _customQueryService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    private ProductDetailModel productDetailModel = null!;

    public Page CurrentPage { get; set; }

    public ProductDetailViewModel(IHttpClientService httpClientService, IProductTransactionService productTransactionService, ICustomQueryService customQueryService, IUserDialogs userDialogs)
    {
        Title = "Ürün Detayı";
        _httpClientService = httpClientService;
        _productTransactionService = productTransactionService;
        _customQueryService = customQueryService;
        _userDialogs = userDialogs;

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        InputQuantityTappedCommand = new Command(async () => await InputQuantityTappedAsync());
        OutputQuantityTappedCommand = new Command(async () => await OutputQuantityTappedAsync());
        ShowProcessBottomSheetCommand = new Command(async () => await ShowProcessBottomSheetAsync());
    }

    #region Commands

    public Command LoadItemsCommand { get; }
    public Command InputQuantityTappedCommand { get; }
    public Command OutputQuantityTappedCommand { get; }

    public Command ShowProcessBottomSheetCommand { get; }

    public Command GoToWarehouseTotalView { get; }

    #endregion Commands

    private async Task LoadItemsAsync()
    {
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            _userDialogs.Loading("Loading Items...");
            await Task.Delay(1000);

            await Task.WhenAll(GetInputOutputQuantityAsync(httpClient), GetLastTransactionsAsync(httpClient));

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

    private async Task GetLastTransactionsAsync(HttpClient httpclient)
    {
        try
        {
            ProductDetailModel.LastTransactions.Clear();

            var result = await _productTransactionService.GetObjects(
                httpClient: httpclient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                productReferenceId: ProductDetailModel.Product.ReferenceId,
                search: string.Empty,
                skip: 0,
                take: 5);

            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                {
                    ProductDetailModel.LastTransactions.Add(Mapping.Mapper.Map<ProductTransaction>(item));
                }
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(message: ex.Message, title: "Hata");
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
}