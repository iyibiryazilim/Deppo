using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DataResultModel;
using Deppo.Core.DTOs.ConsumableTransaction;
using Deppo.Core.DTOs.ProductionTransaction;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.DTOs.TransferTransaction;
using Deppo.Core.ResponseResultModels;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.TransferModels;
using Deppo.Mobile.Core.Models.VirmanModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ResultModule;
using Deppo.Mobile.Modules.ResultModule.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.VirmanProductProcess.ViewModels;

[QueryProperty(name: nameof(VirmanBasketModel), queryId: nameof(VirmanBasketModel))]
public partial class VirmanProductFormListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ITransferTransactionService _transferTransactionService;

    private readonly IProductionTransactionService _productionTransactionService;

    private readonly IConsumableTransactionService _consumableTransactionService;

    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    private VirmanBasketModel virmanBasketModel = null!;

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

    [ObservableProperty]
    private string code = string.Empty;

    public VirmanProductFormListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, ITransferTransactionService transferTransactionService, IProductionTransactionService productionTransactionService, IConsumableTransactionService consumableTransactionService)
    {
        _httpClientService = httpClientService;
        _transferTransactionService = transferTransactionService;
        _userDialogs = userDialogs;

        Title = "Virman Formu";
        SaveCommand = new Command(async () => await SaveAsync());
        BackCommand = new Command(async () => await BackAsync());
        _productionTransactionService = productionTransactionService;
        _consumableTransactionService = consumableTransactionService;
    }

    public Page CurrentPage { get; set; }

    public Command LoadPageCommand { get; }
    public Command BackCommand { get; }
    public Command SaveCommand { get; }

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

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            DataResult<ResponseModel> result = await ConsumableInsert(httpClient);

            if (result.IsSuccess)
            {
                code += result.Data.Code;
                resultModel.Code += "Sarf Numarası :" + code;

                DataResult<ResponseModel> result2 = await ProductionInsert(httpClient);

                if (result2.IsSuccess && result.IsSuccess)
                {
                    code = code + " " + result2.Data.Code;
                    resultModel.Message = "Virman için Üretimden Giriş Ve Sarf Başarılı";
                    resultModel.Code = resultModel.Code + " Üretimden Giriş Numarası" + result2.Data.Code;
                    ClearVirmanBasket();
                    resultModel.PageTitle = Title;
                    resultModel.PageCountToBack = 7;

                    if (_userDialogs.IsHudShowing)
                        _userDialogs.HideHud();

                    await Shell.Current.GoToAsync($"{nameof(InsertSuccessPageView)}", new Dictionary<string, object>
                    {
                        [nameof(ResultModel)] = resultModel
                    });
                }
                else if (result.IsSuccess && !result2.IsSuccess)
                {
                    if (_userDialogs.IsHudShowing)
                        _userDialogs.HideHud();

                    resultModel.Message = "Virman Için Sarf Başarılı Üretimden Giriş Başarısız";
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

    private void ClearVirmanBasket()
    {
        VirmanBasketModel.OutVirmanWarehouse = null;
        VirmanBasketModel.OutVirmanProduct = null;
        VirmanBasketModel.InVirmanWarehouse = null;
        VirmanBasketModel.InVirmanProduct = null;
        VirmanBasketModel.OutVirmanProduct.LocationTransactionModels.Clear();
        VirmanBasketModel.InVirmanProduct.Locations.Clear();
        VirmanBasketModel.OutVirmanQuantity = default;
        VirmanBasketModel.InVirmanQuantity = default;
    }

    private async Task<DataResult<ResponseModel>> ConsumableInsert(HttpClient httpClient)
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
            WarehouseNumber = VirmanBasketModel.OutVirmanWarehouse.Number,
        };

        foreach (var item in VirmanBasketModel.OutVirmanProduct.LocationTransactionModels)
        {
            var consumableTransactionLineDto = new ConsumableTransactionLineDto
            {
                ProductCode = item.ItemCode,
                WarehouseNumber = VirmanBasketModel.OutVirmanWarehouse.Number,
                Quantity = item.Quantity,
                ConversionFactor = 1,
                OtherConversionFactor = 1,
                SubUnitsetCode = VirmanBasketModel.OutVirmanProduct.SubUnitsetCode,
            };

            foreach (var detail in VirmanBasketModel.OutVirmanProduct.LocationTransactionModels)
            {
                if (item.ReferenceId == detail.ReferenceId)
                {
                    var seriLotTransactionDto = new SeriLotTransactionDto
                    {
                        StockLocationCode = detail.LocationCode,
                        Quantity = detail.Quantity,
                        ConversionFactor = 1,
                        OtherConversionFactor = 1,
                        DestinationStockLocationCode = string.Empty,
                        SubUnitsetCode = VirmanBasketModel.OutVirmanProduct.SubUnitsetCode,
                        InProductTransactionLineReferenceId = detail.TransactionReferenceId,
                        OutProductTransactionLineReferenceId = detail.ReferenceId,
                    };

                    consumableTransactionLineDto.SeriLotTransactions.Add(seriLotTransactionDto);
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
            WarehouseNumber = VirmanBasketModel.InVirmanWarehouse.Number,
            Description = Description,
        };

        foreach (var item in VirmanBasketModel.InVirmanProduct.Locations)
        {
            var productionTransactionLineDto = new ProductionTransactionLineDto
            {
                ProductCode = VirmanBasketModel.InVirmanProduct.Code,
                WarehouseNumber = VirmanBasketModel.InVirmanWarehouse.Number,
                Quantity = item.InputQuantity,
                ConversionFactor = 1,
                OtherConversionFactor = 1,
                SubUnitsetCode = VirmanBasketModel.InVirmanProduct.SubUnitsetCode,
            };

            foreach (var detail in VirmanBasketModel.InVirmanProduct.Locations)
            {
                if (item.Code == detail.Code)
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
            }

            productionTransactionDto.Lines.Add(productionTransactionLineDto);
        }

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