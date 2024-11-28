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
using Deppo.Core.BaseModels;
using Deppo.Mobile.Core.Models.CountingModels.BasketModels;
using Deppo.Mobile.Core.Models.PurchaseModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels;

[QueryProperty(name: nameof(ProductDetailModel), queryId: nameof(ProductDetailModel))]
public partial class ProductDetailViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IProductTransactionService _productTransactionService;
    private readonly ICustomQueryService _customQueryService;
    private readonly IUserDialogs _userDialogs;
    private readonly IProductDetailService _productDetailService;
    private readonly ISubUnitsetService _subUnitsetService;

    [ObservableProperty]
    private ProductDetailModel productDetailModel = null!;

    [ObservableProperty]
    private ProductFiche selectedProductFiche;

    [ObservableProperty]
    private ProductMeasure productMeasure = new();

    [ObservableProperty]
    private bool isEdit = false;

    [ObservableProperty]
    private SubUnitset selectedSubunitset;

    public ObservableCollection<ProductDetailActionModel> ProductActionModels { get; } = new();
    public ObservableCollection<SubUnitset> SubUnitsets { get; } = new();

    public Page CurrentPage { get; set; }

    public ProductDetailViewModel(IHttpClientService httpClientService, IProductTransactionService productTransactionService, ICustomQueryService customQueryService, IUserDialogs userDialogs, IProductDetailService productDetailService, ISubUnitsetService subUnitsetService)
    {
        Title = "Ürün Detayı";
        _httpClientService = httpClientService;
        _productTransactionService = productTransactionService;
        _customQueryService = customQueryService;
        _userDialogs = userDialogs;
        _productDetailService = productDetailService;
        _subUnitsetService = subUnitsetService;

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

        SwitchButtonTappedCommand = new Command(async () => await SwitchButtonTappedAsync());
        WidthIncreaseCommand = new Command(async () => await WidthIncreaseAsync());
        WidthDecreaseCommand = new Command(async () => await WidthDecreaseAsync());

        HeightIncreaseCommand = new Command(async () => await HeightIncreaseAsync());
        HeightDecreaseCommand = new Command(async () => await HeightDecreaseAsync());

        LengthIncreaseCommand = new Command(async () => await LengthIncreaseAsync());
        LengthDecreaseCommand = new Command(async () => await LengthDecreaseAsync());

        WeightIncreaseCommand = new Command(async () => await WeightIncreaseAsync());
        WeightDecreaseCommand = new Command(async () => await WeightDecreaseAsync());

        VolumeIncreaseCommand = new Command(async () => await VolumeIncreaseAsync());
        VolumeDecreaseCommand = new Command(async () => await VolumeDecreaseAsync());

        ImageOptionCommand = new Command(async () => await ImageOptionAsync());

        OpenInfoBottemSheetCommand = new Command(async () => await OpenInfoBottemSheetAsync());

        UpdateProductMeasureCommand = new Command(async () => await UpdateProductMeasureAsync());
        ItemSubunitsetTappedCommand = new Command<SubUnitset>(ItemTappedAsync);
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

    public Command OpenInfoBottemSheetCommand { get; }

    //ActionModel

    //Üç Nokta
    public Command ActionModelProcessTappedCommand { get; }

    //Tıkladığımı Diğer Sayfaya Göndereceğim
    public Command ActionModelsTappedCommand { get; }

    public Command ItemTappedCommand { get; }
    public Command SwitchButtonTappedCommand { get; }

    public Command WidthIncreaseCommand { get; }

    public Command WidthDecreaseCommand { get; }

    public Command HeightIncreaseCommand { get; }

    public Command HeightDecreaseCommand { get; }

    public Command LengthIncreaseCommand { get; }

    public Command LengthDecreaseCommand { get; }

    public Command WeightIncreaseCommand { get; }

    public Command WeightDecreaseCommand { get; }

    public Command VolumeIncreaseCommand { get; }

    public Command VolumeDecreaseCommand { get; }

    #endregion Commands

    public Command TakePictureCommand { get; set; }
    public Command ImageOptionCommand { get; set; }
    public Command CaptureImageCommand { get; set; }
    public Command PickImageCommand { get; set; }
    public Command ClearImageCommand { get; set; }
    public Command UpdateProductMeasureCommand { get; set; }

    public Command ItemSubunitsetTappedCommand { get; set; }

    private async Task LoadItemsAsync()
    {
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            _userDialogs.Loading("Loading Items...");
            await Task.Delay(1000);

            await Task.WhenAll(GetInputQuantityAsync(), GetOutputQuantityAsync(), GetLastTransactionsAsync(), GetProductInputOutputQuantitiesAsync(), CalculateTurnoverRateAsync());

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

    //Devir hızı dönem başı stok miktarı
    private async Task GetFirstStockQuantityAsync()
    {
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var result = await _productDetailService.GetFirstStockQuantityAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, ProductDetailModel.Product.ReferenceId);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var obj = Mapping.Mapper.Map<InventoryTurnover>(item);
                    ProductDetailModel.InventoryTurnover.FirstStockQuantity = obj.FirstStockQuantity;
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

    //Devir Hızı Son stok miktarı
    private async Task GetLastStockQuantityAsync()
    {
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var result = await _productDetailService.GetLastStockQuantityAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, ProductDetailModel.Product.ReferenceId);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var obj = Mapping.Mapper.Map<InventoryTurnover>(item);
                    ProductDetailModel.InventoryTurnover.LastStockQuantity = obj.LastStockQuantity;
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

    // Devir hızı satış miktarı
    private async Task GetSalesQuantityAsync()
    {
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var result = await _productDetailService.GetSalesQuantityAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, ProductDetailModel.Product.ReferenceId);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;
                foreach (var item in result.Data)
                {
                    var obj = Mapping.Mapper.Map<InventoryTurnover>(item);
                    ProductDetailModel.InventoryTurnover.SalesQuantity = obj.SalesQuantity;
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

    //private async Task GetPurchaseQuantityAsync()
    //{
    //    if (IsBusy)
    //        return;
    //    try
    //    {
    //        IsBusy = true;

    //        var httpClient = _httpClientService.GetOrCreateHttpClient();

    //        foreach (var item in ProductDetailModel.PurchaseInventoryTurnovers)
    //        {
    //            var result = await _productDetailService.GetPurchaseQuantityAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, ProductDetailModel.Product.ReferenceId, item.Month);

    //            if (result.IsSuccess)
    //            {
    //                if (result.Data is null)
    //                    return;

    //                item.StockQuantity = Convert.ToInt32(result.Data);
    //            }
    //        }

    //        foreach (var item in ProductDetailModel.PurchaseInventoryTurnovers)
    //        {
    //            var result = await _productDetailService.GetAvarageStockQuantityAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, ProductDetailModel.Product.ReferenceId, item.Month);

    //            if (result.IsSuccess)
    //            {
    //                if (result.Data is null)
    //                    return;

    //                item.StockQuantity = Convert.ToInt32(result.Data);
    //            }
    //        }

    //    }
    //    catch (Exception ex)
    //    {
    //        if (_userDialogs.IsHudShowing)
    //            _userDialogs.HideHud();

    //        _userDialogs.Alert(ex.Message, "Hata", "Tamam");
    //    }
    //    finally
    //    {
    //        IsBusy = false;
    //    }
    //}

    private async Task CalculateTurnoverRateAsync()
    {
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            await GetFirstStockQuantityAsync();
            await GetLastStockQuantityAsync();
            await GetSalesQuantityAsync();

            var avarageStockQuantity = (ProductDetailModel.InventoryTurnover.FirstStockQuantity + ProductDetailModel.InventoryTurnover.LastStockQuantity) / 2;
            ProductDetailModel.InventoryTurnover.TurnoverRate = ProductDetailModel.InventoryTurnover.SalesQuantity / (avarageStockQuantity == 0 ? 1 : avarageStockQuantity);
            if (ProductDetailModel.InventoryTurnover.TurnoverRate < 0)
                ProductDetailModel.InventoryTurnover.TurnoverRate = 0;
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
            var result = await _productDetailService.ProductInputOutputQuantities(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, DateTime.Now, ProductDetailModel.Product.ReferenceId);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                List<ProductDetailInputOutputModel> cacheItems = new();

                foreach (var item in result.Data)
                {
                    var value = Mapping.Mapper.Map<ProductDetailInputOutputModel>(item);
                    cacheItems.Add(value);
                }

                ProductDetailModel.ProductInputOutputModels.Clear();
                foreach (var item in cacheItems.OrderBy(x => x.ArgumentDay))
                    ProductDetailModel.ProductInputOutputModels.Add(item);
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

            Console.WriteLine(ProductDetailModel);

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
			ProductActionModels.Add(new ProductDetailActionModel
			{
				ActionName = "Alternatif Malzemeler",
				ActionUrl = $"{nameof(ProductDetailAlternativeProductListView)}",
				LineNumber = 6,
				Icon = "",
				IsSelected = false
			});

			if (ProductDetailModel.Product.IsPurchased)
            {
                ProductActionModels.Add(new ProductDetailActionModel
                {
                    ActionName = "Onaylı Tedarikçiler",
                    ActionUrl = $"{nameof(ProductDetailApprovedSupplierView)}",
                    LineNumber = 7,
                    Icon = "",
                    IsSelected = false
                });
            }

            ProductActionModels.Add(new ProductDetailActionModel
            {
                ActionName = "Malzeme Ölçüleri",
                ActionUrl = $"",
                LineNumber = 8,
                Icon = "",
                IsSelected = false
            });
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
            if (model.ActionName != "Malzeme Ölçüleri")
            {
                await Shell.Current.GoToAsync($"{model.ActionUrl}", new Dictionary<string, object>
                {
                    [nameof(ProductDetailModel)] = ProductDetailModel
                });
            }
            else
            {
                await GetProductMeasuresAsync();

                CurrentPage.FindByName<BottomSheet>("measureBottomSheet").State = BottomSheetState.HalfExpanded;

                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();
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

            SelectedProductFiche = productFiche;

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

    private async Task SwitchButtonTappedAsync()
    {
        IsEdit = !IsEdit;
        LoadSubUnitsetsAsync();
    }

    private async Task WidthIncreaseAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (ProductMeasure is not null)
            {
                ProductMeasure.Width++;
            }
            else
            {
                _userDialogs.Alert("İşlem Yapılamadı", "Tamam");
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

    private async Task WidthDecreaseAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (ProductMeasure is not null)
            {
                if (ProductMeasure.Width > 0)
                    ProductMeasure.Width--;
            }
            else
            {
                _userDialogs.Alert("İşlem Yapılamadı", "Tamam");
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

    private async Task HeightIncreaseAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (ProductMeasure is not null)
            {
                ProductMeasure.Height++;
            }
            else
            {
                _userDialogs.Alert("İşlem Yapılamadı", "Tamam");
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

    private async Task HeightDecreaseAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (ProductMeasure is not null)
            {
                if (ProductMeasure.Height > 0)
                    ProductMeasure.Height--;
            }
            else
            {
                _userDialogs.Alert("İşlem Yapılamadı", "Tamam");
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

    private async Task LengthIncreaseAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (ProductMeasure is not null)
            {
                ProductMeasure.Length++;
            }
            else
            {
                _userDialogs.Alert("İşlem Yapılamadı", "Tamam");
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

    private async Task LengthDecreaseAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (ProductMeasure is not null)
            {
                if (ProductMeasure.Length > 0)
                    ProductMeasure.Length--;
            }
            else
            {
                _userDialogs.Alert("İşlem Yapılamadı", "Tamam");
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

    private async Task WeightIncreaseAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (ProductMeasure is not null)
            {
                ProductMeasure.Weight++;
            }
            else
            {
                _userDialogs.Alert("İşlem Yapılamadı", "Tamam");
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

    private async Task WeightDecreaseAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (ProductMeasure is not null)
            {
                if (ProductMeasure.Weight > 0)
                    ProductMeasure.Weight--;
            }
            else
            {
                _userDialogs.Alert("İşlem Yapılamadı", "Tamam");
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

    private async Task VolumeIncreaseAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (ProductMeasure is not null)
            {
                ProductMeasure.Volume++;
            }
            else
            {
                _userDialogs.Alert("İşlem Yapılamadı", "Tamam");
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

    private async Task VolumeDecreaseAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (ProductMeasure is not null)
            {
                if (ProductMeasure.Volume > 0)
                    ProductMeasure.Volume--;
            }
            else
            {
                _userDialogs.Alert("İşlem Yapılamadı", "Tamam");
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

    private async Task ImageOptionAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(500);
            var result = await _userDialogs.ActionSheetAsync(
                            message: "",
                            title: "Malzeme Resmi",
                            cancel: "Vazgeç",
                            destructive: "Temizle",
                            icon: null,
                            useBottomSheet: true,
                            cancelToken: default,
                            "Kamerayı Kullan",
                            "Kütüphane"
                            );

            if (result == "Kamerayı Kullan")
            {
                await CaptureImageAsync();
            }
            else if (result == "Kütüphane")
            {
                await PickImageAsync();
            }
            else if (result == "Temizle")
            {
                //await ClearImageAsync();
            }
            else
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();
            }

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

    private async Task CaptureImageAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(ProductCaptureImageView)}", new Dictionary<string, object>
        {
            [nameof(Product)] = ProductDetailModel.Product
        });
    }

    private async Task PickImageAsync()
    {
        FileResult? photo = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
        {
            Title = "Select your photo"
        });

        if (photo is not null)
        {
            var stream = await photo.OpenReadAsync();

            Image productImage = CurrentPage.FindByName<Image>("productImage");
            if (productImage is not null)
                productImage.Source = ImageSource.FromStream(() => stream);

            //update or insert image
        }
    }

    private async Task OpenInfoBottemSheetAsync()
    {
        try
        {
            IsBusy = true;

            CurrentPage.FindByName<BottomSheet>("ınfoBottomSheet").State = BottomSheetState.HalfExpanded;
        }
        catch (System.Exception)
        {
            _userDialogs.Alert("Bir hata oluştu. Lütfen tekrar deneyiniz.", "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task UpdateProductMeasureAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _productDetailService.UpdateProductMeasure(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, productReferenceId: ProductDetailModel.Product.ReferenceId, width: ProductMeasure.Width, height: ProductMeasure.Height, length: ProductMeasure.Length, weight: ProductMeasure.Weight, volume: ProductMeasure.Volume, subunitsetId: SelectedSubunitset.ReferenceId);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;
                foreach (var item in result.Data)
                {
                    ProductMeasure = Mapping.Mapper.Map<ProductMeasure>(item);
                }
                _userDialogs.Alert("Bilgi", "Güncelleme başarılı!");
            }
            else
            {
                _userDialogs.Alert("Uyarı", "Güncelleme işlemi başarısız...");
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

    private async Task LoadSubUnitsetsAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            _userDialogs.Loading("Yükleniyor");
            await Task.Delay(1000);
            SubUnitsets.Clear();
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _subUnitsetService.GetObjects(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, productReferenceId: ProductDetailModel.Product.ReferenceId);
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                    SubUnitsets.Add(Mapping.Mapper.Map<SubUnitset>(item));

                _userDialogs.Loading().Hide();
            }
            else
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.Loading().Hide();

                _userDialogs.Alert(message: result.Message, title: "Load Items");
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(message: ex.Message, title: "Load Items Error");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void ItemTappedAsync(SubUnitset subUnitset)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            var selectedItem = SubUnitsets.FirstOrDefault(x => x.ReferenceId == subUnitset.ReferenceId);
            if (selectedItem != null)

                SelectedSubunitset = subUnitset;
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