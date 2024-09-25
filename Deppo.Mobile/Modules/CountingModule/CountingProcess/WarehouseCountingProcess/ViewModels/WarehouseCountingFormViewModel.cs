using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DTOs.ConsumableTransaction;
using Deppo.Core.DTOs.InCountingTransaction;
using Deppo.Core.DTOs.OutCountingTransaction;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.DTOs.WastageTransaction;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.CountingModels;
using Deppo.Mobile.Core.Models.CountingModels.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ResultModule;
using Deppo.Mobile.Modules.ResultModule.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using static Android.Util.EventLogTags;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.ViewModels;

[QueryProperty(nameof(LocationModel), nameof(LocationModel))]
[QueryProperty(nameof(WarehouseCountingWarehouseModel), nameof(WarehouseCountingWarehouseModel))]
[QueryProperty(nameof(WarehouseCountingBasketModel), nameof(WarehouseCountingBasketModel))]
public partial class WarehouseCountingFormViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IInCountingTransactionService _inCountingTransactionService;
    private readonly IOutCountingTransactionService _outCountingTransactionService;
    private readonly IUserDialogs _userDialogs;



    [ObservableProperty]
    LocationModel locationModel = null!;

    [ObservableProperty]
    WarehouseCountingWarehouseModel warehouseCountingWarehouseModel = null!;

    [ObservableProperty]
    ObservableCollection<WarehouseCountingBasketModel> warehouseCountingBasketModel = null!;

    [ObservableProperty]
    ObservableCollection<WarehouseCountingBasketModel> inputItems = new();

    [ObservableProperty]
    ObservableCollection<WarehouseCountingBasketModel> outputItems = new();


    [ObservableProperty]
    string documentNumber = string.Empty;

    [ObservableProperty]
    DateTime transactionDate = DateTime.Now;

    [ObservableProperty]
    string description = string.Empty;

    [ObservableProperty]
    string documentTrackingNumber = string.Empty;

    [ObservableProperty]
    string specialCode = string.Empty;


    public WarehouseCountingFormViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IOutCountingTransactionService outCountingTransactionService, IInCountingTransactionService inCountingTransactionService)
    {
        _httpClientService = httpClientService;
        _inCountingTransactionService = inCountingTransactionService;
        _outCountingTransactionService = outCountingTransactionService;
        _userDialogs = userDialogs;

        SaveCommand = new Command(async () => await SaveAsync());
        LoadPageCommand = new Command(async () => await LoadPageAsync());
        ShowBasketItemCommand = new Command(async () => await ShowBasketItemAsync());
    }
    public Page CurrentPage { get; set; }

    public Command SaveCommand { get; }
    public Command LoadPageCommand { get; }
    public Command ShowBasketItemCommand { get; }
    public Command BackCommand { get; }

    private async Task LoadPageAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            //Title = GetEnumDescription(OutputProductProcessType);
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



    public static string GetEnumDescription(Enum value)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());

        DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

        if (attributes != null && attributes.Any())
        {
            return attributes.First().Description;
        }

        return value.ToString();
    }


    private async Task SaveAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var confirm = await _userDialogs.ConfirmAsync("Fiş oluşturulacaktır. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
            if (!confirm)
                return;

            foreach (var item in WarehouseCountingBasketModel)
            {
                if (item.OutputQuantity - item.StockQuantity > 0)
                    InputItems.Add(item);
                else
                    OutputItems.Add(item);

            }

            _userDialogs.ShowLoading("İşlem Tamamlanıyor...");
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            if (OutputItems.Count > 0)
            {
                await OutCountingTransactionInsert(httpClient);
            }

            if (InputItems.Count > 0)
            {
                await InCountingTransactionInsert(httpClient);
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


    private async Task OutCountingTransactionInsert(HttpClient httpClient)
    {
        var outCountingTransactionDto = new OutCountingTransactionInsert
        {
            Code = "",
            CurrentCode = "",
            Description = Description,
            DoCode = DocumentNumber,
            DocTrackingNumber = DocumentTrackingNumber,
            TransactionDate = TransactionDate,
            FirmNumber = _httpClientService.FirmNumber,
            SpeCode = SpecialCode,
            WarehouseNumber = WarehouseCountingWarehouseModel.Number,

        };

        foreach (var item in OutputItems)
        {
            var outCountingTransactionLineDto = new OutCountingTransactionLineDto
            {
                ProductCode = item.ProductCode,
                WarehouseNumber = WarehouseCountingWarehouseModel.Number,
                Quantity = item.StockQuantity - item.OutputQuantity,
                ConversionFactor = 1,
                OtherConversionFactor = 1,
                SubUnitsetCode = item.SubUnitsetCode,
            };

            if (item.LocationTransactions.Count > 0)
            {
                foreach (var detail in item.LocationTransactions)
                {
                    var serilotTransactionDto = new SeriLotTransactionDto
                    {
                        StockLocationCode = detail.LocationCode,
                        InProductTransactionLineReferenceId = detail.TransactionReferenceId,
                        OutProductTransactionLineReferenceId = detail.ReferenceId,
                        Quantity = detail.OutputQuantity,
                        SubUnitsetCode = item.SubUnitsetCode,
                        DestinationStockLocationCode = string.Empty,
                        ConversionFactor = 1,
                        OtherConversionFactor = 1,
                    };

                    outCountingTransactionLineDto.SeriLotTransactions.Add(serilotTransactionDto);
                }
            }


            outCountingTransactionDto.Lines.Add(outCountingTransactionLineDto);
        }

        var result = await _outCountingTransactionService.InsertOutCountingTransaction(httpClient, outCountingTransactionDto, _httpClientService.FirmNumber);

        ResultModel resultModel = new();
        if (result.IsSuccess)
        {
            resultModel.Message = "Başarılı";
            resultModel.Code = result.Data.Code;
            resultModel.PageTitle = Title;
            resultModel.PageCountToBack = 4;

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

            resultModel.Message = "Başarısız";
            resultModel.PageTitle = Title;
            resultModel.PageCountToBack = 1;
            await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
            {
                [nameof(ResultModel)] = resultModel
            });
        }
    }

    private async Task InCountingTransactionInsert(HttpClient httpClient)
    {
        var inCountingTransactionDto = new InCountingTransactionInsert
        {
            Code = "",
            CurrentCode = "",
            Description = Description,
            DoCode = DocumentNumber,
            DocTrackingNumber = DocumentTrackingNumber,
            TransactionDate = TransactionDate,
            FirmNumber = _httpClientService.FirmNumber,
            SpeCode = SpecialCode,
            WarehouseNumber = WarehouseCountingWarehouseModel.Number,

        };

        foreach (var item in InputItems)
        {
            var inCountingTransactionLineDto = new InCountingTransactionLineDto
            {
                ProductCode = item.ProductCode,
                WarehouseNumber = WarehouseCountingWarehouseModel.Number,
                Quantity = item.OutputQuantity - item.StockQuantity,
                ConversionFactor = 1,
                OtherConversionFactor = 1,
                SubUnitsetCode = item.SubUnitsetCode,
            };

            inCountingTransactionDto.Lines.Add(inCountingTransactionLineDto);
        }

        var result = await _inCountingTransactionService.InsertInCountingTransaction(httpClient, inCountingTransactionDto, _httpClientService.FirmNumber);

        ResultModel resultModel = new();
        if (result.IsSuccess)
        {
            resultModel.Message = "Başarılı";
            resultModel.Code = result.Data.Code;
            resultModel.PageTitle = Title;
            resultModel.PageCountToBack = 4;

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

            resultModel.Message = "Başarısız";
            resultModel.PageTitle = Title;
            resultModel.PageCountToBack = 1;
            await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
            {
                [nameof(ResultModel)] = resultModel
            });
        }
    }
}
