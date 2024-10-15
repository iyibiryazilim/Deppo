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

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrder.ViewModels
{
    [QueryProperty(name: nameof(QuicklyBomProductBasketModel), queryId: nameof(QuicklyBomProductBasketModel))]
    public partial class WorkOrderFormViewModel : BaseViewModel
    {
        private readonly IUserDialogs _userDialogs;
        private readonly IHttpClientService _httpClientService;
        private readonly IConsumableTransactionService _consumableTransactionService;
        private readonly IProductionTransactionService _productionTransactionService;

        [ObservableProperty]
        private QuicklyBomProductBasketModel quicklyBomProductBasketModel = null!;

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

        public WorkOrderFormViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IConsumableTransactionService consumableTransactionService, IProductionTransactionService productionTransactionService)
        {
            Title = "Form Sayfası";

            _httpClientService = httpClientService;
            _userDialogs = userDialogs;

            LoadPageCommand = new Command(async () => await LoadPageAsync());
            ShowBasketItemCommand = new Command(async () => await ShowBasketItemAsync());
            SaveCommand = new Command(async () => await SaveAsync());
            _consumableTransactionService = consumableTransactionService;
            _productionTransactionService = productionTransactionService;
        }

        public Page CurrentPage { get; set; }

        public Command LoadPageCommand { get; }
        public Command BackCommand { get; }
        public Command SaveCommand { get; }
        public Command ShowBasketItemCommand { get; }

        public async Task ShowBasketItemAsync()
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
                                .GroupBy(x => x.ProductModel.WarehouseNumber)
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
                        DocumentNumber = string.Empty;
                        ficheDate = DateTime.Now;
                        DocumentTrackingNumber = string.Empty;
                        SpecialCode = string.Empty;
                        Description = string.Empty;
                        Cleans();

                        resultModel.PageCountToBack = 4;

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
                        resultModel.PageCountToBack = 1;
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
          //  QuicklyBomProductBasketModel.SubProducts.Select(x => x.ProductModel).ToList().Clear();
          //  QuicklyBomProductBasketModel.SubProducts.Select(x => x.LocationTransactions).ToList().Clear();
            QuicklyBomProductBasketModel.SubProducts.Clear();
            QuicklyBomProductBasketModel.QuicklyBomProduct = null;
            QuicklyBomProductBasketModel.BOMQuantity = 0;
            QuicklyBomProductBasketModel.MainAmount = 0;
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
                WarehouseNumber = quicklyBomSubProductModel.FirstOrDefault().ProductModel.WarehouseNumber,
            };

            foreach (var item in quicklyBomSubProductModel)
            {
                var consumableTransactionLineDto = new ConsumableTransactionLineDto
                {
                    ProductCode = item.ProductModel.IsVariant ? item.ProductModel.MainProductCode : item.ProductModel.Code,
                    VariantCode = item.ProductModel.IsVariant ? item.ProductModel.Code : "",
					WarehouseNumber = quicklyBomSubProductModel.FirstOrDefault().ProductModel.WarehouseNumber,
                    Quantity = item.SubBOMQuantity,
                    ConversionFactor = 1,
                    OtherConversionFactor = 1,
                    SubUnitsetCode = item.ProductModel.SubUnitsetCode,
                };

                foreach (var detail in item.LocationTransactions)
                {
                    var serilotTransactionDto = new SeriLotTransactionDto
                    {
                        StockLocationCode = detail.LocationCode,
                        InProductTransactionLineReferenceId = detail.TransactionReferenceId,
                        OutProductTransactionLineReferenceId = detail.ReferenceId,
                        Quantity = detail.OutputQuantity,
                        SubUnitsetCode = item.ProductModel.SubUnitsetCode,
                        DestinationStockLocationCode = string.Empty,
                        ConversionFactor = 1,
                        OtherConversionFactor = 1,
                    };

                    consumableTransactionLineDto.SeriLotTransactions.Add(serilotTransactionDto);
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
				VariantCode = QuicklyBomProductBasketModel.QuicklyBomProduct.IsVariant ? QuicklyBomProductBasketModel.QuicklyBomProduct.Code : "",
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
    }
}