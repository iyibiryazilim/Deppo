using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DTOs.ConsumableTransaction;
using Deppo.Core.DTOs.ProductionTransaction;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.DTOs.TransferTransaction;
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
    VirmanBasketModel virmanBasketModel= null!;

    [ObservableProperty]
    DateTime ficheDate = DateTime.Now;


    [ObservableProperty]
    DateTime transactionDate = DateTime.Now;

    [ObservableProperty]
    string documentNumber = string.Empty;

    [ObservableProperty]
    string documentTrackingNumber = string.Empty;

    [ObservableProperty]
    string specialCode = string.Empty;

    [ObservableProperty]
    string description = string.Empty;



    public VirmanProductFormListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, ITransferTransactionService transferTransactionService, IProductionTransactionService productionTransactionService, IConsumableTransactionService consumableTransactionService)
    {
        _httpClientService = httpClientService;
        _transferTransactionService = transferTransactionService;
        _userDialogs = userDialogs;

        Title = "Virman Formu";
        SaveCommand = new Command(async () => await SaveAsync());
        _productionTransactionService = productionTransactionService;
        _consumableTransactionService = consumableTransactionService;
    }

    public Page CurrentPage { get; set; }

    public Command LoadPageCommand { get; }
    public Command BackCommand { get; }
    public Command SaveCommand { get; }
    public Command ShowBasketItemCommand { get; }

    


    private async Task SaveAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("İşlem Tamamlanıyor...");
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();


            var consumableTransactionDto = new ConsumableTransactionInsert
            {
                Code = "",
                CurrentCode = "",
                Description = Description,
                DoCode = DocumentNumber,
                DocTrackingNumber = DocumentTrackingNumber,
                TransactionDate = TransactionDate,
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
                    SubUnitsetCode = item.SubUnitsetCode,
                };

                foreach (var detail in VirmanBasketModel.OutVirmanProduct.LocationTransactionModels)
                {
                    if (item.ReferenceId == detail.ReferenceId)
                    {
                        var seriLotTransactionDto = new SeriLotTransactionDto
                        {
                            StockLocationCode = detail.LocationCode,
                            Quantity = detail.OutputQuantity,
                            ConversionFactor = 1,
                            OtherConversionFactor = 1,
                            DestinationStockLocationCode = string.Empty,
                        };

                        consumableTransactionLineDto.SeriLotTransactions.Add(seriLotTransactionDto);
                    }

                }
                consumableTransactionDto.Lines.Add(consumableTransactionLineDto);
            }

            var result = await _consumableTransactionService.InsertConsumableTransaction(httpClient, consumableTransactionDto, _httpClientService.FirmNumber);



















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
                    if(item.Code == detail.Code)
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


            ResultModel resultModel = new();
            if (result.IsSuccess)
            {
                resultModel.Message = "Sarf Başarılı";
                resultModel.Code = result.Data.Code;
                resultModel.PageTitle = Title;
                resultModel.PageCountToBack = 1;

                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

                await Shell.Current.GoToAsync($"{nameof(InsertSuccessPageView)}", new Dictionary<string, object>
                {
                    [nameof(ResultModel)] = resultModel
                });
            }
            else
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

                resultModel.Message = "Sarf Başarısız";
                resultModel.PageTitle = Title;
                resultModel.PageCountToBack = 1;
                await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
                {
                    [nameof(ResultModel)] = resultModel
                });
            }


            if (result2.IsSuccess)
            {
                resultModel.Message = "Üretimden Giriş Başarılı";
                resultModel.Code = result2.Data.Code;
                resultModel.PageTitle = Title;
                resultModel.PageCountToBack = 7;

                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

                await Shell.Current.GoToAsync($"{nameof(InsertSuccessPageView)}", new Dictionary<string, object>
                {
                    [nameof(ResultModel)] = resultModel
                });
            }
            else
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

                resultModel.Message = "Üretimden Giriş Başarısız";
                resultModel.PageTitle = Title;
                resultModel.PageCountToBack = 7;
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



}