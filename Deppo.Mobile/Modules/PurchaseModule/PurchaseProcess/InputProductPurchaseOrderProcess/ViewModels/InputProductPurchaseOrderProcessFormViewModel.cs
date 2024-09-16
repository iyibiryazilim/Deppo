using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DTOs.PurchaseDispatchTransaction;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
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

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(PurchaseSupplier), queryId: nameof(PurchaseSupplier))]
[QueryProperty(name: nameof(Items), queryId: nameof(Items))]
public partial class InputProductPurchaseOrderProcessFormViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly IPurchaseDispatchTransactionService _purchaseDispatchTransactionService;

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
    private PurchaseSupplier purchaseSupplier;

    [ObservableProperty]
    private ObservableCollection<InputPurchaseBasketModel> items = null!;

    public InputProductPurchaseOrderProcessFormViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IPurchaseDispatchTransactionService purchaseDispatchTransactionService)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _purchaseDispatchTransactionService = purchaseDispatchTransactionService;
        Items = new();

        Title = "Siparişe Bağlı Mal Kabul İşlemi";

        LoadPageCommand = new Command(async () => await LoadPageAsync());
        ShowBasketItemCommand = new Command(async () => await ShowBasketItemAsync());
        SaveCommand = new Command(async () => await SaveAsync());
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

            var purchaseDispatchDto = new PurchaseDispatchTransactionInsert
            {
                SpeCode = SpecialCode,
                CurrentCode = PurchaseSupplier.Code,
                Code = string.Empty,
                DocTrackingNumber = DocumentTrackingNumber,
                DoCode = DocumentNumber,
                TransactionDate = FicheDate,
                FirmNumber = _httpClientService.FirmNumber,
                WarehouseNumber = WarehouseModel.Number,
                Description = Description
            };

            foreach (var item in items.Where(x => x.Orders.Count > 0))
            {
                var remainingQuantity = item.InputQuantity;
                foreach (var orders in item.Orders.OrderBy(x => x.OrderDate))
                {
                    if (remainingQuantity > 0)
                    {
                        var purchaseDispatchTransactionLineDto = new PurchaseDispatchTransactionLineDto
                        {
                            ProductCode = item.ItemCode,
                            OrderReferenceId = orders.OrderReferenceId,
                            WarehouseNumber = (short)WarehouseModel.Number,
                            Quantity = remainingQuantity > orders.WaitingQuantity ? orders.WaitingQuantity : remainingQuantity,
                            ConversionFactor = 1,
                            OtherConversionFactor = 1,
                            SubUnitsetCode = item.SubUnitsetCode,
                        };

                        foreach (var detail in item.Details)
                        {
                            var seriLotTransactionDto = new SeriLotTransactionDto
                            {
                                StockLocationCode = detail.LocationCode,
                                SubUnitsetCode = item.SubUnitsetCode,
                                Quantity = Convert.ToDouble(remainingQuantity > orders.WaitingQuantity ? orders.WaitingQuantity : remainingQuantity),
                                ConversionFactor = 1,
                                OtherConversionFactor = 1,
                                DestinationStockLocationCode = string.Empty,
                            };

                            purchaseDispatchTransactionLineDto.SeriLotTransactions.Add(seriLotTransactionDto);
                        }

                        purchaseDispatchDto.Lines.Add(purchaseDispatchTransactionLineDto);

                        remainingQuantity -= Convert.ToDouble(remainingQuantity > orders.WaitingQuantity ? orders.WaitingQuantity : remainingQuantity);
                    }
                    else
                    {
                        break;
                    }
                }

                if (remainingQuantity > 0)
                {
                    var purchaseDispatchTransactionLineDto = new PurchaseDispatchTransactionLineDto
                    {
                        ProductCode = item.ItemCode,
                        OrderReferenceId = null,
                        WarehouseNumber = (short)WarehouseModel.Number,
                        Quantity = remainingQuantity,
                        ConversionFactor = 1,
                        OtherConversionFactor = 1,
                        SubUnitsetCode = item.SubUnitsetCode,
                        SpeCode = "Sipariş Fazlası",
                        Description = string.Empty,
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

                        purchaseDispatchTransactionLineDto.SeriLotTransactions.Add(seriLotTransactionDto);
                    }

                    purchaseDispatchDto.Lines.Add(purchaseDispatchTransactionLineDto);
                    remainingQuantity -= remainingQuantity;
                }
            }

            foreach (var item in items.Where(x => x.Orders.Count == 0))
            {
                var purchaseDispatchTransactionLineDto = new PurchaseDispatchTransactionLineDto
                {
                    ProductCode = item.ItemCode,
                    OrderReferenceId = item.Orders[0].OrderReferenceId,
                    WarehouseNumber = (short)WarehouseModel.Number,
                    Quantity = item.InputQuantity,
                    ConversionFactor = 1,
                    OtherConversionFactor = 1,
                    SubUnitsetCode = item.SubUnitsetCode,
                };

                foreach (var detail in item.Details)
                {
                    var seriLotTransactionDto = new SeriLotTransactionDto
                    {
                        StockLocationCode = detail.LocationCode,
                        SubUnitsetCode = item.SubUnitsetCode,
                        Quantity = item.InputQuantity,
                        ConversionFactor = 1,
                        OtherConversionFactor = 1,
                        DestinationStockLocationCode = string.Empty,
                    };

                    purchaseDispatchTransactionLineDto.SeriLotTransactions.Add(seriLotTransactionDto);
                }

                purchaseDispatchDto.Lines.Add(purchaseDispatchTransactionLineDto);
            }

            var result = await _purchaseDispatchTransactionService.InsertPurchaseDispatchTransaction(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, purchaseDispatchDto);

            Console.WriteLine(result);
            ResultModel resultModel = new();
            if (result.IsSuccess)
            {
                resultModel.Message = "Başarılı";
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