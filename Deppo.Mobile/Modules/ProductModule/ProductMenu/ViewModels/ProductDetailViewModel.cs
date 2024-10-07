using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.ActionModels.ProductActionModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductMenu.Views;
using Deppo.Mobile.Modules.ProductModule.ProductMenu.Views.ActionViews;
using DevExpress.Maui.Controls;
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

    [ObservableProperty]
    private ProductMeasure productMeasure = new();

    public ObservableCollection<ProductDetailActionModel> ProductActionModels { get; } = new();



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

        GetLastTransactionsCommand = new Command(async () => await GetLastTransactionsAsync());
        AllFicheTappedCommand = new Command(async () => await AllFicheTappedAsync());
        ItemTappedCommand = new Command<ProductFiche>(async (productFiche) => await ItemTappedAsync(productFiche));

        GetLastTransactionCommand = new Command<ProductFiche>(async (productFiche) => await GetLastTransactionAsync(productFiche));

        ActionModelsTappedCommand = new Command<ProductDetailActionModel>(async (model) => await ActionModelsTappedAsync(model));

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

            await Task.WhenAll(GetInputQuantityAsync(),GetOutputQuantityAsync(), GetLastTransactionsAsync(),GetProductMeasuresAsync(), GetProductInputOutputQuantitiesAsync(),CalculateTurnoverRateAsync());

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

    private async Task SetMonthsAsync()
    {
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            int currentMonth = DateTime.Now.Month;

            if (ProductDetailModel.SalesInventoryTurnovers.Count > 0)
                ProductDetailModel.SalesInventoryTurnovers.Clear();

            if (ProductDetailModel.PurchaseInventoryTurnovers.Count > 0)
                ProductDetailModel.PurchaseInventoryTurnovers.Clear();

            for (int i = 0; i < 6; i++)
            {
                int monthToSet = currentMonth - i;

                if (monthToSet <= 0)
                    break;


                var salesTurnover = new SalesInventoryTurnover();
                salesTurnover.Month = monthToSet;

                var purchaseTurnover = new PurchaseInventoryTurnover();
                purchaseTurnover.Month = monthToSet;

                ProductDetailModel.SalesInventoryTurnovers.Add(salesTurnover);
                ProductDetailModel.PurchaseInventoryTurnovers.Add(purchaseTurnover);
            }
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

    private async Task GetAvarageStockQuantityAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            foreach (var item in ProductDetailModel.SalesInventoryTurnovers)
            {
                var result = await _productDetailService.GetAvarageStockQuantityAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, ProductDetailModel.Product.ReferenceId, item.Month);

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    item.StockQuantity = Convert.ToInt32(result.Data);
                }
            }

            foreach (var item in ProductDetailModel.PurchaseInventoryTurnovers)
            {
                var result = await _productDetailService.GetAvarageStockQuantityAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, ProductDetailModel.Product.ReferenceId, item.Month);

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    item.StockQuantity = Convert.ToInt32(result.Data);
                }
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
            IsBusy = false;
        }
    }

    private async Task GetSalesQuantityAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            foreach (var item in ProductDetailModel.SalesInventoryTurnovers)
            {
                var result = await _productDetailService.GetSalesQuantityAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, ProductDetailModel.Product.ReferenceId, item.Month);

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    item.StockQuantity = Convert.ToInt32(result.Data);
                }
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
            IsBusy = false;
        }
    }

    private async Task GetPurchaseQuantityAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            foreach (var item in ProductDetailModel.PurchaseInventoryTurnovers)
            {
                var result = await _productDetailService.GetPurchaseQuantityAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, ProductDetailModel.Product.ReferenceId, item.Month);

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    item.StockQuantity = Convert.ToInt32(result.Data);
                }
            }

            foreach (var item in ProductDetailModel.PurchaseInventoryTurnovers)
            {
                var result = await _productDetailService.GetAvarageStockQuantityAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, ProductDetailModel.Product.ReferenceId, item.Month);

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    item.StockQuantity = Convert.ToInt32(result.Data);
                }
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
            IsBusy = false;
        }
    }

    private async Task CalculateTurnoverRateAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            await SetMonthsAsync();
            await Task.WhenAll(GetAvarageStockQuantityAsync(), GetSalesQuantityAsync(), GetPurchaseQuantityAsync());

            foreach (var salesItem in ProductDetailModel.SalesInventoryTurnovers)
            {
                if (salesItem.StockQuantity > 0)
                    salesItem.TurnoverRate = (double)salesItem.SalesQuantity / salesItem.StockQuantity;
                else
                    salesItem.TurnoverRate = 0;

            }

            foreach (var purchaseItem in ProductDetailModel.PurchaseInventoryTurnovers)
            {
                if (purchaseItem.StockQuantity > 0)
                    purchaseItem.TurnoverRate = (double)purchaseItem.PurchaseQuantity / purchaseItem.StockQuantity;
                else
                    purchaseItem.TurnoverRate = 0;

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
            IsBusy = false;
        }
    }


    private async Task GetInputQuantityAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _productDetailService.GetInputQuantity(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, ProductDetailModel.Product.ReferenceId);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var obj = Mapping.Mapper.Map<ProductDetailModel>(item);
                    ProductDetailModel.InputQuantity = obj.InputQuantity;
                }
                    
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
    private async Task GetOutputQuantityAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _productDetailService.GetOutputQuantity(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, ProductDetailModel.Product.ReferenceId);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var obj = Mapping.Mapper.Map<ProductDetailModel>(item);
                    ProductDetailModel.OutputQuantity = obj.OutputQuantity;
                }

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

    private async Task GetProductInputOutputQuantitiesAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _productDetailService.ProductInputOutputQuantities(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, DateTime.Now,ProductDetailModel.Product.ReferenceId);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                ProductDetailModel.ProductInputOutputModels.Clear();
                foreach (var item in result.Data)
                    ProductDetailModel.ProductInputOutputModels.Add(Mapping.Mapper.Map<ProductDetailInputOutputModel>(item));
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

    private async Task GetProductMeasuresAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _productDetailService.GetProductMeasure(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, ProductDetailModel.Product.ReferenceId);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;
                foreach (var item in result.Data)
                {
                    ProductMeasure = Mapping.Mapper.Map<ProductMeasure>(item);
                }
                
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

            ProductActionModels.Add(new ProductDetailActionModel
            {
                ActionName = "Ambar Toplamları",
                ActionUrl = $"{nameof(ProductDetailWarehouseTotalListView)}",
                LineNumber = 1,
                Icon = "",
                IsSelected = false
            });


            if (ProductDetailModel.Product.IsVariant)
            {
                ProductActionModels.Add(new ProductDetailActionModel
                {
                    ActionName = "Varyantlar",
                    ActionUrl = $"{nameof(ProductDetailVariantListView)}",
                    LineNumber = 2,
                    Icon = "",
                    IsSelected = false
                });
            }
            if (ProductDetailModel.Product.LocTracking == 1)
            {
                ProductActionModels.Add(new ProductDetailActionModel
                {
                    ActionName = "Stok Yeri Dağılımı",
                    ActionUrl = $"{nameof(ProductDetailLocationTransactionListView)}",
                    LineNumber = 3,
                    Icon = "",
                    IsSelected = false
                });
            }

            ProductActionModels.Add(new ProductDetailActionModel
            {
                ActionName = "Bekleyen Satış Siparişleri",
                ActionUrl = $"{nameof(ProductDetailWaitingSalesOrderListView)}",
                LineNumber = 4,
                Icon = "",
                IsSelected = false
            });

            ProductActionModels.Add(new ProductDetailActionModel
            {
                ActionName = "Bekleyen Satınalma Siparişleri",
                ActionUrl = $"{nameof(ProductDetailWaitingPurchaseOrderListView)}",
                LineNumber = 5,
                Icon = "",
                IsSelected = false
            });

            if (ProductDetailModel.Product.IsPurchased)
            {
                ProductActionModels.Add(new ProductDetailActionModel
                {
                    ActionName = "Onaylı Tedarikçiler",
                    ActionUrl = $"{nameof(ProductDetailApprovedSupplierView)}",
                    LineNumber = 6,
                    Icon = "",
                    IsSelected = false
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

    private async Task ActionModelsTappedAsync(ProductDetailActionModel model)
    {
        try
        {
            IsBusy = true;
            CurrentPage.FindByName<BottomSheet>("processBottomSheet").State = BottomSheetState.Hidden;
            await Task.Delay(300);
            await Shell.Current.GoToAsync($"{model.ActionUrl}", new Dictionary<string, object>
            {
                [nameof(ProductDetailModel)] = ProductDetailModel
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