using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DataResultModel;
using Deppo.Core.ResponseResultModels;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.QuicklyModels.BasketModels;
using Deppo.Mobile.Core.Models.VirmanModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ResultModule.Views;
using Deppo.Mobile.Modules.ResultModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deppo.Core.DTOs.ConsumableTransaction;
using Deppo.Core.DTOs.ProductionTransaction;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Mobile.Core.Models.QuicklyModels;
using DevExpress.Maui.Controls;
using Deppo.Core.Models;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.MappingHelper;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.ViewModels;

[QueryProperty(name: nameof(QuicklyBomProductBasketModel), queryId: nameof(QuicklyBomProductBasketModel))]
public partial class ManuelFormListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ITransferTransactionService _transferTransactionService;

    private readonly IProductionTransactionService _productionTransactionService;

    private readonly IConsumableTransactionService _consumableTransactionService;
    private readonly ILocationTransactionService _locationTransactionService;

    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    private QuicklyBomProductBasketModel quicklyBomProductBasketModel = null!;

    public ObservableCollection<LocationTransactionModel> LocationTransactions { get; } = new();

    [ObservableProperty]
    private DateTime ficheDate = DateTime.Now;

    [ObservableProperty]
    private string documentNumber = string.Empty;

    [ObservableProperty]
    private string documentTrackingNumber = string.Empty;

    [ObservableProperty]
    private string specialCode = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    public ManuelFormListViewModel(IHttpClientService httpClientService, ITransferTransactionService transferTransactionService, IProductionTransactionService productionTransactionService, IConsumableTransactionService consumableTransactionService, IUserDialogs userDialogs, ILocationTransactionService locationTransactionService)
    {
        Title = "Form Sayfası";

        LoadPageCommand = new Command(async () => await LoadPageAsync());
        ShowBasketItemCommand = new Command(async () => await ShowBasketItemAsync());
        SaveCommand = new Command(async () => await SaveAsync());
        BackCommand = new Command(async () => await BackAsync());

        _httpClientService = httpClientService;
        _transferTransactionService = transferTransactionService;
        _productionTransactionService = productionTransactionService;
        _consumableTransactionService = consumableTransactionService;
        _userDialogs = userDialogs;
        _locationTransactionService = locationTransactionService;
    }

    public Page CurrentPage { get; set; }

    public Command LoadPageCommand { get; }
    public Command BackCommand { get; }
    public Command SaveCommand { get; }
    public Command ShowBasketItemCommand { get; }

    private async Task ShowBasketItemAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            CurrentPage.FindByName<BottomSheet>("basketItemBottomSheet").State = BottomSheetState.HalfExpanded;
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

    private async Task LoadPageAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            _userDialogs.Loading("Loading");
            //await Task.Delay(750);
            CurrentPage.FindByName<BottomSheet>("basketItemBottomSheet").State = BottomSheetState.HalfExpanded;

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

    private async Task SaveAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("İşlem Tamamlanıyor...");
            await Task.Delay(1000);
            ResultModel resultModel = new();
            var result = new DataResult<ResponseModel>();
            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var groupList = QuicklyBomProductBasketModel.SubProducts
                            .GroupBy(x => x.WarehouseModel.Number)
                            .ToList();

            int count = groupList.Count;
            int Sayac = 0;
            resultModel.Code = "Sarf Numaraları:";
            foreach (var group in groupList)
            {
                // Grubu listeye dönüştürüp metot çağrısı yapıyoruz

                result = await ConsumableInsert(httpClient, group.ToList());
                if (result.IsSuccess)
                {
                    Sayac = Sayac + 1;
                    resultModel.Code += " " + result.Data.Code + " ,";
                }
                else
                {
                    break;
                }
            }

            if (result.IsSuccess && Sayac == count)
            {
                DataResult<ResponseModel> result2 = await ProductionInsert(httpClient);

                if (result2.IsSuccess)
                {
                    resultModel.Code += " Üretimden Giriş Numarası: " + result2.Data.Code;
                    resultModel.Message = "Hızlı üretim için Üretimden Giriş Ve Sarf Başarılı";
                    resultModel.PageTitle = Title;
                    resultModel.PageCountToBack = 7;
                    QuicklyBomProductBasketModel.QuicklyBomProduct = null;
                    QuicklyBomProductBasketModel.WarehouseNumber = default;
                    Cleans();

                    if (_userDialogs.IsHudShowing)
                        _userDialogs.HideHud();

                    await Shell.Current.GoToAsync($"{nameof(InsertSuccessPageView)}", new Dictionary<string, object>
                    {
                        [nameof(ResultModel)] = resultModel
                    });
                }
                else if (Sayac == count && !result2.IsSuccess)
                {
                    if (_userDialogs.IsHudShowing)
                        _userDialogs.HideHud();

                    resultModel.Message = "Hızlı Üretim Için Sarf Başarılı Üretimden Giriş Başarısız";
                    resultModel.Code = resultModel.Code;

                    resultModel.PageTitle = Title;
                    resultModel.PageCountToBack = 7;
                    await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
                    {
                        [nameof(ResultModel)] = resultModel
                    });
                }
            }
            else
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

                resultModel.Message = "Virman Başarısız";
                resultModel.PageTitle = "Virman İşlemi";
                resultModel.PageCountToBack = 1;
                resultModel.ErrorMessage = result.Message;

                await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
                {
                    [nameof(ResultModel)] = resultModel
                });
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

    private void Cleans()
    {
        QuicklyBomProductBasketModel.MainLocations.Clear();
        QuicklyBomProductBasketModel.SubProducts.Clear();
        QuicklyBomProductBasketModel.QuicklyBomProduct = null;
        QuicklyBomProductBasketModel.BOMQuantity = 0;
        QuicklyBomProductBasketModel.MainAmount = 0;
    }

    public async Task LoadLocationTransaction(QuicklyBomSubProductModel quicklyBomSubProduct, GroupLocationTransactionModel groupLocationTransactionModel)
    {
        try
        {

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            LocationTransactions.Clear();
			var result = await _locationTransactionService.GetInputObjectsAsync(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                productReferenceId: quicklyBomSubProduct.ProductModel.IsVariant ? quicklyBomSubProduct.ProductModel.MainProductReferenceId : quicklyBomSubProduct.ProductModel.ReferenceId,
                variantReferenceId: quicklyBomSubProduct.ProductModel.IsVariant ? quicklyBomSubProduct.ProductModel.ReferenceId : 0,
                warehouseNumber: quicklyBomSubProduct.WarehouseModel.Number,
                locationRef: groupLocationTransactionModel.LocationReferenceId,
                skip: 0,
                take: 999999,
                search: ""
            );

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;
                foreach (var item in result.Data)
                {
                    LocationTransactions.Add(Mapping.Mapper.Map<LocationTransactionModel>(item));
                }

            }

            _userDialogs.Loading().Hide();
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


    private async Task<DataResult<ResponseModel>> ConsumableInsert(HttpClient httpClient, List<QuicklyBomSubProductModel> quicklyBomSubProductModel)
    {
        var consumableTransactionDto = new ConsumableTransactionInsert
        {
            Code = "",
            CurrentCode = "",
            Description = Description,
            DoCode = DocumentNumber,
            DocTrackingNumber = DocumentTrackingNumber,
            TransactionDate = FicheDate,
            FirmNumber = _httpClientService.FirmNumber,
            SpeCode = SpecialCode,
            WarehouseNumber = quicklyBomSubProductModel.FirstOrDefault().WarehouseModel.Number,
        };

        foreach (var item in quicklyBomSubProductModel)
        {
            var tempItemQuantity = item.SubBOMQuantity;
			var consumableTransactionLineDto = new ConsumableTransactionLineDto
            {
                ProductCode = item.ProductModel.IsVariant ? item.ProductModel.MainProductCode : item.ProductModel.Code,
                VariantCode = item.ProductModel.IsVariant ? item.ProductModel.Code : "",
                WarehouseNumber = quicklyBomSubProductModel.FirstOrDefault()?.WarehouseModel.Number,
                Quantity = item.SubBOMQuantity,
                ConversionFactor = 1,
                OtherConversionFactor = 1,
                SubUnitsetCode = item.ProductModel.SubUnitsetCode,
            };

            foreach (var detail in item.LocationTransactions)
            {
                var tempDetailQuantity = detail.OutputQuantity;
				await LoadLocationTransaction(item, detail);
                LocationTransactions.OrderBy(x => x.TransactionDate).ToList();

                foreach (var locationTransaction in LocationTransactions)
                {
					var tempLocationTransactionQuantity = locationTransaction.RemainingQuantity;

					while (tempLocationTransactionQuantity > 0 && tempItemQuantity > 0 && tempDetailQuantity>0)
                    {
						var serilotTransactionDto = new SeriLotTransactionDto
                        {
                            StockLocationCode = detail.LocationCode,
                            InProductTransactionLineReferenceId = locationTransaction.TransactionReferenceId,
                            OutProductTransactionLineReferenceId = locationTransaction.ReferenceId,
                            Quantity = tempDetailQuantity > tempLocationTransactionQuantity ? tempLocationTransactionQuantity : tempDetailQuantity,
                            SubUnitsetCode = item.ProductModel.SubUnitsetCode,
                            DestinationStockLocationCode = string.Empty,
                            ConversionFactor = 1,
                            OtherConversionFactor = 1,
                        };
                        consumableTransactionLineDto.SeriLotTransactions.Add(serilotTransactionDto);
						tempLocationTransactionQuantity -= (double)serilotTransactionDto.Quantity;
						tempDetailQuantity -= (double)serilotTransactionDto.Quantity;
						tempItemQuantity -= (double)serilotTransactionDto.Quantity;
					}
                }

            }
            consumableTransactionDto.Lines.Add(consumableTransactionLineDto);
        }

        var result = await _consumableTransactionService.InsertConsumableTransaction(httpClient, consumableTransactionDto, _httpClientService.FirmNumber);
        return result;
    }

    private async Task<DataResult<ResponseModel>> ProductionInsert(HttpClient httpClient)
    {
        var productionTransactionDto = new ProductionTransactionInsert
        {
            SpeCode = SpecialCode,
            CurrentCode = string.Empty,
            Code = string.Empty,
            DocTrackingNumber = DocumentTrackingNumber,
            DoCode = DocumentNumber,
            TransactionDate = FicheDate,
            FirmNumber = _httpClientService.FirmNumber,
            WarehouseNumber = QuicklyBomProductBasketModel.WarehouseNumber,
            Description = Description,
        };

        var productionTransactionLineDto = new ProductionTransactionLineDto
        {
            ProductCode = QuicklyBomProductBasketModel.QuicklyBomProduct.IsVariant ? QuicklyBomProductBasketModel.QuicklyBomProduct.MainItemCode : QuicklyBomProductBasketModel.QuicklyBomProduct.Code,
            VariantCode = QuicklyBomProductBasketModel.QuicklyBomProduct.IsVariant ? QuicklyBomProductBasketModel.QuicklyBomProduct.Code : string.Empty,
            WarehouseNumber = QuicklyBomProductBasketModel.WarehouseNumber,
            Quantity = QuicklyBomProductBasketModel.BOMQuantity,
            ConversionFactor = 1,
            OtherConversionFactor = 1,
            SubUnitsetCode = QuicklyBomProductBasketModel.QuicklyBomProduct.SubUnitsetCode,
            UnitPrice = 0,
            VatRate = 0,
        };

        foreach (var detail in QuicklyBomProductBasketModel.MainLocations)
        {
            var seriLotTransactionDto = new SeriLotTransactionDto
            {
                StockLocationCode = detail.Code,
                Quantity = detail.InputQuantity,
                ConversionFactor = 1,
                OtherConversionFactor = 1,
                DestinationStockLocationCode = string.Empty,
            };

            productionTransactionLineDto.SeriLotTransactions.Add(seriLotTransactionDto);
        }

        productionTransactionDto.Lines.Add(productionTransactionLineDto);

        var result2 = await _productionTransactionService.InsertProductionTransaction(httpClient, productionTransactionDto, _httpClientService.FirmNumber);
        return result2;
    }

    private async Task BackAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var result = await _userDialogs.ConfirmAsync("Form verileri silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
            if (!result)
                return;

            await ClearFormAsync();

            await Shell.Current.GoToAsync("..");
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

    private async Task ClearFormAsync()
    {
        try
        {
            DocumentNumber = string.Empty;
            SpecialCode = string.Empty;
            DocumentTrackingNumber = string.Empty;
            Description = string.Empty;
        }
        catch (Exception ex)
        {
            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
    }
}