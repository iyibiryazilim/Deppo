using Android.App;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DataResultModel;
using Deppo.Core.DTOs.PurchaseDispatchTransaction;
using Deppo.Core.DTOs.RetailSalesReturnDispatchTransaction;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.DTOs.WholeSalesReturnDispatchTransaction;
using Deppo.Core.ResponseResultModels;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ResultModule;
using Deppo.Mobile.Modules.ResultModule.Views;
using DevExpress.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesDispatchProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(SalesCustomer), queryId: nameof(SalesCustomer))]
[QueryProperty(name: nameof(Items), queryId: nameof(Items))]
public partial class ReturnSalesDispatchFormViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly IRetailSalesReturnDispatchTransactionService _retailService;
    private readonly IWholeSalesReturnDispatchTransactionService _wholeService;

    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

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
    private SalesCustomer salesCustomer;

    [ObservableProperty]
    private ObservableCollection<ReturnSalesBasketModel> items = null!;

    [ObservableProperty]
    private SalesReturnEnumType salesReturnEnumType = SalesReturnEnumType.Retail;

    public ReturnSalesDispatchFormViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IRetailSalesReturnDispatchTransactionService retailSalesDispatchTransactionService, IWholeSalesReturnDispatchTransactionService wholeSalesReturnDispatchTransactionService)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        Items = new();

        Title = "Form Sayfası";

        LoadPageCommand = new Command(async () => await LoadPageAsync());
        ShowBasketItemCommand = new Command(async () => await ShowBasketItemAsync());

        SelectWholeCommand = new Command(async (x) => await SelectTransactionTypeAsync(SalesReturnEnumType.Whole));
        SelectRetailCommand = new Command(async (x) => await SelectTransactionTypeAsync(SalesReturnEnumType.Retail));

        SaveCommand = new Command(OpenBottomSheetAsync);

        _retailService = retailSalesDispatchTransactionService;
        _wholeService = wholeSalesReturnDispatchTransactionService;
    }

    public Page CurrentPage { get; set; }

    public Command LoadPageCommand { get; }
    public Command BackCommand { get; }
    public Command SaveCommand { get; }
    public Command ShowBasketItemCommand { get; }

    public Command SelectWholeCommand { get; }
    public Command SelectRetailCommand { get; }

    private async Task ShowBasketItemAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            CurrentPage.FindByName<BottomSheet>("basketItemBottomSheet").State = BottomSheetState.HalfExpanded;
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

    private async Task LoadPageAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            // Title = GetEnumDescription(InputProductProcessType);
            CurrentPage.FindByName<BottomSheet>("basketItemBottomSheet").State = BottomSheetState.HalfExpanded;
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

    private void OpenBottomSheetAsync()
    {
        // BottomSheet'i aç
        CurrentPage.FindByName<BottomSheet>("selectConfirmBottomSheet").State = BottomSheetState.HalfExpanded;
    }

    private async Task SelectTransactionTypeAsync(SalesReturnEnumType selectedType)
    {
        SalesReturnEnumType = selectedType;

        // BottomSheet'i kapat
        CurrentPage.FindByName<BottomSheet>("selectConfirmBottomSheet").State = BottomSheetState.Hidden;

        // SaveCommand'i tetikle
        await SaveAsync();
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
            _userDialogs.ShowLoading("İşlem Tamamlanıyor...");
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            DataResult<ResponseModel> result = new();

            if (salesReturnEnumType == SalesReturnEnumType.Retail)
            {
                RetailSalesReturnDispatchTransactionInsert dto = new()
                {
                    SpeCode = SpecialCode,
                    CurrentCode = SalesCustomer.Code,
                    Code = string.Empty,
                    DocTrackingNumber = DocumentTrackingNumber,
                    DoCode = DocumentNumber,
                    TransactionDate = FicheDate,
                    FirmNumber = _httpClientService.FirmNumber,
                    WarehouseNumber = WarehouseModel.Number,
                    Description = Description
                };

                foreach (var item in Items)
                {
                    var remainingQuantity = item.Quantity;

                    foreach (var orders in item.Dispatches.OrderBy(x => x.OrderDate))
                    {
                        if (remainingQuantity > 0)
                        {
                            var line = new RetailSalesReturnDispatchTransactionLineInsert
                            {
                                ProductCode = item.ItemCode,
                                WarehouseNumber = (short)WarehouseModel.Number,
                                Quantity = remainingQuantity > orders.WaitingQuantity ? orders.WaitingQuantity : remainingQuantity,
                                ConversionFactor = 1,
                                OtherConversionFactor = 1,
                                SubUnitsetCode = item.SubUnitsetCode,
                                DispatchReferenceId = orders.ReferenceId,
                                Description = Description,
                                OrderReferenceId = orders.ReferenceId,
                                SpeCode = SpecialCode,
                            };

                            dto.Lines.Add(line);

                            foreach (var detail in item.Details)
                            {
                                var seriLotTransactionDto = new SeriLotTransactionDto
                                {
                                    StockLocationCode = detail.LocationCode,
                                    SubUnitsetCode = item.SubUnitsetCode,
                                    Quantity = remainingQuantity > detail.Quantity ? detail.Quantity : remainingQuantity,
                                    ConversionFactor = 1,
                                    OtherConversionFactor = 1,
                                    DestinationStockLocationCode = string.Empty,
                                };

                                line.SeriLotTransactions.Add(seriLotTransactionDto);
                            }

                            remainingQuantity -= Convert.ToDouble(remainingQuantity > orders.WaitingQuantity ? orders.WaitingQuantity : remainingQuantity);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (remainingQuantity > 0)
                    {
                        var line = new RetailSalesReturnDispatchTransactionLineInsert
                        {
                            ProductCode = item.ItemCode,
                            WarehouseNumber = (short)WarehouseModel.Number,
                            Quantity = remainingQuantity,
                            ConversionFactor = 1,
                            OtherConversionFactor = 1,
                            SubUnitsetCode = item.SubUnitsetCode,
                            Description = "Sipariş Fazlası",
                        };

                        foreach (var detail in item.Details)
                        {
                            var seriLotTransactionDto = new SeriLotTransactionDto
                            {
                                StockLocationCode = detail.LocationCode,
                                SubUnitsetCode = item.SubUnitsetCode,
                                Quantity = remainingQuantity,
                                ConversionFactor = 1,
                                OtherConversionFactor = 1,
                                DestinationStockLocationCode = string.Empty,
                            };

                            line.SeriLotTransactions.Add(seriLotTransactionDto);
                        }

                        dto.Lines.Add(line);
                    }
                }

                result = await _retailService.InsertRetailSalesReturnDispatchTransaction(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, dto);
            }
            else
            {
                WholeSalesReturnDispatchTransactionInsert dto = new()
                {
                    SpeCode = SpecialCode,
                    CurrentCode = SalesCustomer.Code,
                    Code = string.Empty,
                    DocTrackingNumber = DocumentTrackingNumber,
                    DoCode = DocumentNumber,
                    TransactionDate = FicheDate,
                    FirmNumber = _httpClientService.FirmNumber,
                    WarehouseNumber = WarehouseModel.Number,
                    Description = Description
                };

                foreach (var item in Items)
                {
                    var remainingQuantity = item.Quantity;

                    foreach (var orders in item.Dispatches.OrderBy(x => x.OrderDate))
                    {
                        if (remainingQuantity > 0)
                        {
                            var wholeSalesDispatchTransactionLineInsert = new WholeSalesReturnTransactionLineInsert
                            {
                                ProductCode = item.ItemCode,
                                WarehouseNumber = (short)WarehouseModel.Number,
                                Quantity = remainingQuantity > orders.WaitingQuantity ? orders.WaitingQuantity : remainingQuantity,
                                ConversionFactor = 1,
                                OtherConversionFactor = 1,
                                SubUnitsetCode = orders.SubUnitsetCode,
                                DispatchReferenceId = orders.ReferenceId,
                                Description = Description,
                                OrderReferenceId = orders.ReferenceId,
                                SpeCode = SpecialCode,
                            };

                            dto.Lines.Add(wholeSalesDispatchTransactionLineInsert);

                            foreach (var detail in item.Details)
                            {
                                var seriLotTransactionDto = new SeriLotTransactionDto
                                {
                                    StockLocationCode = detail.LocationCode,
                                    SubUnitsetCode = item.SubUnitsetCode,
                                    Quantity = remainingQuantity > detail.Quantity ? detail.Quantity : remainingQuantity,
                                    ConversionFactor = 1,
                                    OtherConversionFactor = 1,
                                    DestinationStockLocationCode = string.Empty,
                                };

                                wholeSalesDispatchTransactionLineInsert.SeriLotTransactions.Add(seriLotTransactionDto);
                            }

                            remainingQuantity -= Convert.ToDouble(remainingQuantity > orders.WaitingQuantity ? orders.WaitingQuantity : remainingQuantity);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (remainingQuantity > 0)
                    {
                        var wholeSalesDispatchTransactionLineInsert = new WholeSalesReturnTransactionLineInsert
                        {
                            ProductCode = item.ItemCode,
                            WarehouseNumber = (short)WarehouseModel.Number,
                            Quantity = remainingQuantity,
                            ConversionFactor = 1,
                            OtherConversionFactor = 1,
                            SubUnitsetCode = item.SubUnitsetCode,
                            Description = "Sipariş Fazlası",
                            //DispatchReferenceId = item.Dispatches.FirstOrDefault().ReferenceId,
                            SpeCode = SpecialCode,
                        };

                        foreach (var detail in item.Details)
                        {
                            var seriLotTransactionDto = new SeriLotTransactionDto
                            {
                                StockLocationCode = detail.LocationCode,
                                SubUnitsetCode = item.SubUnitsetCode,
                                Quantity = remainingQuantity,
                                ConversionFactor = 1,
                                OtherConversionFactor = 1,
                                DestinationStockLocationCode = string.Empty,
                            };

                            wholeSalesDispatchTransactionLineInsert.SeriLotTransactions.Add(seriLotTransactionDto);
                        }

                        dto.Lines.Add(wholeSalesDispatchTransactionLineInsert);
                    }
                }

                result = await _wholeService.InsertWholeSalesReturnDispatchTransaction(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, dto);
            }
            Console.WriteLine(result);
            ResultModel resultModel = new();
            if (result.IsSuccess)
            {
                resultModel.Message = "Başarılı" + result.Message;
                resultModel.Code = result.Data.Code;
                resultModel.PageTitle = Title;
                resultModel.PageCountToBack = 5;

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
                resultModel.PageCountToBack = 4;
                await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
                {
                    [nameof(ResultModel)] = resultModel
                });
            }
        }
        catch (Exception ex)
        {
            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
        finally { IsBusy = false; }
    }
}